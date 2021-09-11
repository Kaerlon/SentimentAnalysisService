using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.ML;
using SentimentAnalysis.MlNet.Model;
using SentimentAnalysis.API.Models;
using Microsoft.EntityFrameworkCore;
using SentimentAnalysis.API.Options;

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
                conf.UseSqlServer(Configuration.GetConnectionString("Default"), sqlConf =>
                 {
                     sqlConf.CommandTimeout(60);
                     sqlConf.EnableRetryOnFailure(30);
                     sqlConf.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                 });
            });

            var mlConf = Configuration.GetSection(nameof(MLConfiguration)).Get<MLConfiguration>();

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

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
