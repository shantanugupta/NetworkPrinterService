using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PrinterService;
using Serilog;
using System.IO;
using System;

internal class Program
{
    private static void Main(string[] args)
    {
        var host = CreateHostBuilder(args);
        InitializeSeriLog();
        Log.Information("Starting main");
        host.Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>

         Host.CreateDefaultBuilder(args)
            .UseWindowsService() // Run as a Windows Service
            .ConfigureServices((hostContext, services) =>
            {
                services.AddCors(options =>
                {
                    options.AddPolicy("AllowAllOriginsWithCredentials", builder =>
                    {
                        builder.SetIsOriginAllowed(origin => true) // Allow all origins dynamically
                               .AllowAnyHeader()
                               .AllowAnyMethod()
                               .AllowCredentials(); // This requires explicit origins or dynamic logic
                    });
                });

                services.AddHttpContextAccessor();
                services.AddSignalR();
                services.AddSingleton<IJobManager, JobManager>();
                services.AddSingleton<IUserIdProvider, AarfidUserIdProvider>();
                services.AddHostedService<Worker>();
                
            }).ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseUrls("http://0.0.0.0:5000"); // Specify the URL and port
                webBuilder.Configure(app =>
                {
                    //app.UseCors("AllowSpecificOrigins");
                    app.UseCors("AllowAllOriginsWithCredentials");

                    app.UseRouting();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapHub<PrintHub>("/hubs/print"); // Configure SignalR hub endpoint
                    });
                });
            });

    private static void InitializeSeriLog()
    {
        //https://github.com/serilog/serilog/wiki/Enrichment
        //Install-Package Serilog.Enrichers.Environment -Version 3.0.1
        //Install-Package Serilog.Enrichers.Process
        //Install-Package Serilog.Enrichers.Thread - This can be used to enrich thread level data. e.g. processing happening at the gate or readpoint
        //Install-Package Serilog.Exceptions -Version 8.4.0
        //Install - Package Serilog.Enrichers.ClientInfo
        try
        {

            //var environmentBasedFile = $"serilog.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json";
            var environmentBasedFile = $"serilog.json";

            var configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               //.AddJsonFile("serilog.json")
               .AddJsonFile(environmentBasedFile, optional: true, reloadOnChange: true)
               .Build();

            Log.Logger = new LoggerConfiguration()
                        .ReadFrom.Configuration(configuration)
                        .CreateLogger();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}