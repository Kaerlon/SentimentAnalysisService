using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SentimentAnalysis.API.Options;

namespace SentimentAnalysis.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureAppConfiguration(conf =>
                    {
                        conf.AddJsonFile("appsettings.MLConfiguration.json", false, true);
                    });

                    webBuilder.ConfigureServices(services =>
                    {
                        services.AddOptions<AnalyzeOption>();
                    })

                    webBuilder.UseStartup<Startup>();
                });
    }
}
