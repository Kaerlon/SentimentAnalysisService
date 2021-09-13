using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using SentimentAnalysis.Bot.Data;
using SentimentAnalysis.Bot.Extensions;
using SentimentAnalysis.Bot.Options;

using Telegram.Bot.Advanced.Extensions;

namespace SentimentAnalysis.Bot
{
	public class Startup
	{
		public Startup(IConfiguration configuration, IWebHostEnvironment env)
		{
			Configuration = configuration;
			HostingEnvironment = env;
		}

		public readonly IConfiguration Configuration;
		public readonly IWebHostEnvironment HostingEnvironment;

		public ILogger _logger;
		private AppSettings _appSettings;

		public void ConfigureServices(IServiceCollection services)
		{
			var serviceProvider = services.BuildServiceProvider();
			_logger = serviceProvider.GetService<ILogger<Startup>>();

			_appSettings = Configuration.GetSection(nameof(AppSettings))
				.Get<AppSettings>();

			services.AddOptions();
			services.Configure<AppSettings>(Configuration.GetSection(nameof(AppSettings)));

			services.AddMemoryCache();
			services.AddDbContext<ApplicationContext>(options =>
			{
				options.EnableDetailedErrors();
				options.EnableSensitiveDataLogging();
				options.EnableServiceProviderCaching();

				options.UseSqlite(Configuration.GetConnectionString("SqlLite"),
									 opt => { opt.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery); }
									);
			});

			services.AddHttpClient("MLHttpClient", options =>
			{
				_logger.LogDebug("ML service URI = {0}", _appSettings.MLOptions.ServiceUri);
				options.BaseAddress = _appSettings.MLOptions.ServiceUri;
			});

			services.AddTelegramBots(_appSettings);

			services.AddResponseCaching();
			services.AddResponseCompression(options =>
			{
				options.EnableForHttps = true;
				options.Providers.Add<BrotliCompressionProvider>();
				options.Providers.Add<GzipCompressionProvider>();
			});

			services.AddControllers();
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				//app.UseTelegramRouting(new TelegramRoutingOptions()
				//{
				//	WebhookBaseUrl = _appSettings.Telegram.WebhookBaseUrl.ToString()
				//});
			}

			app.UseTelegramPolling();

			app.UseResponseCaching();
			app.UseResponseCompression();

			app.UseRouting();
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
