using FunctionApp.Models.DAO;
using FunctionApp.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication(
        app =>
        {

        }
    )
    .ConfigureServices((context, service) =>
    {
        service.AddApplicationInsightsTelemetryWorkerService();
        service.ConfigureFunctionsApplicationInsights();

        // DbContext 
        service.AddDbContext<MyDbContext>(option =>
        option.UseSqlServer(
            context.Configuration.GetConnectionString("MyDatabase"))
        );

        // Dependency Injection
        service.AddHttpClient();
        service.AddSingleton<IItemService, ItemService>();

    })
    .ConfigureLogging(logging =>
    {
        logging.AddFilter("System.Net.Http.HttpClient", LogLevel.Warning);
    })
    .Build();


host.Run();