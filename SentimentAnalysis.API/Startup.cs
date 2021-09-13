using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.ML;
using Microsoft.OpenApi.Models;
using SentimentAnalysis.API.Models;
using SentimentAnalysis.API.Options;
using SentimentAnalysis.MlNet.Model;

namespace SentimentAnalysis.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SentimentAnalysis.API", Version = "v1" });
            });

            services.AddDbContext<TrainModelContext>(conf =>
            {
                conf.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                conf.UseSqlite(Configuration.GetConnectionString("SqlLite"), sqlConf =>
                 {
                     sqlConf.CommandTimeout(60);
                     sqlConf.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                 });
            });

            var mlConf = Configuration.GetSection(nameof(MLConfiguration)).Get<MLConfiguration>();

            services.AddOptions();
            services.Configure<MLConfiguration>(Configuration.GetSection(nameof(MLConfiguration)));

            services.AddPredictionEnginePool<SentimentData, SentimentPrediction>()
                    .FromFile(mlConf.ModelName, mlConf.FilePath, mlConf.WatchForChanges);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SentimentAnalysis.API v1"));
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
