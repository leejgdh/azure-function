//using Microsoft.Azure.Functions.Worker;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;

//var host = new HostBuilder()
//    .ConfigureFunctionsWebApplication()
//    .ConfigureServices(services =>
//    {
//        services.AddApplicationInsightsTelemetryWorkerService();
//        services.ConfigureFunctionsApplicationInsights();
//    })
//    .Build();

//host.Run();

using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication(
        app =>
        {
            
        }
    )
    .ConfigureServices(service =>
    {
        service.AddApplicationInsightsTelemetryWorkerService();
        service.ConfigureFunctionsApplicationInsights();

        service.AddHttpClient();

        //s.AddSingleton<IHttpResponderService, DefaultHttpResponderService>();
        
        //s.Configure<LoggerFilterOptions>(options =>
        //{
        //    // The Application Insights SDK adds a default logging filter that instructs ILogger to capture only Warning and more severe logs. Application Insights requires an explicit override.
        //    // Log levels can also be configured using appsettings.json. For more information, see https://learn.microsoft.com/en-us/azure/azure-monitor/app/worker-service#ilogger-logs
        //    LoggerFilterRule toRemove = options.Rules.FirstOrDefault(rule => rule.ProviderName
        //        == "Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider");

        //    if (toRemove is not null)
        //    {
        //        options.Rules.Remove(toRemove);
        //    }
        //});
    })
    .ConfigureLogging(logging =>
    {
        logging.AddFilter("System.Net.Http.HttpClient", LogLevel.Warning);

        logging.Services.Configure<LoggerFilterOptions>(options =>
        {
            LoggerFilterRule defaultRule = options.Rules.FirstOrDefault(rule => rule.ProviderName
                == "Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider");
            if (defaultRule is not null)
            {
                options.Rules.Remove(defaultRule);
            }
        });
    })
    .Build();


host.Run();