using FunctionApp.Models.DAO;
using FunctionApp.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Configurations;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication(
        builder =>
        {
            builder.UseFunctionsAuthorization();
        }
    )
    .ConfigureServices((context, service) =>
    {
        service.AddApplicationInsightsTelemetryWorkerService();
        service.ConfigureFunctionsApplicationInsights();

        #region Swagger

        service.AddSingleton<IOpenApiConfigurationOptions>(_ =>
        {
            var options = new OpenApiConfigurationOptions()
            {
                Info = new OpenApiInfo()
                {
                    Version = "1.0.0",
                    Title = "Azure function default",
                    Description = "this is hwida item api",
                    TermsOfService = new Uri("https://github.com/Azure/azure-functions-openapi-extension"),
                    Contact = new OpenApiContact()
                    {
                        Name = "leejgdh",
                        Email = "leejgdh@hwida.com",
                        Url = new Uri("https://github.com/Azure/azure-functions-openapi-extension/issues"),
                    },
                    License = new OpenApiLicense()
                    {
                        Name = "MIT",
                        Url = new Uri("http://opensource.org/licenses/MIT"),
                    }
                },
                Servers = DefaultOpenApiConfigurationOptions.GetHostNames(),
                //to use authroize must v3
                OpenApiVersion = OpenApiVersionType.V3,
                IncludeRequestingHostName = true,
                ForceHttps = false,
                ForceHttp = false,
            };

            return options;
        });

        service
           .AddFunctionsAuthentication(JwtBearerDefaults.AuthenticationScheme)
           // or just ASP.NET Core's .AddAuthentication(... 
           .AddJwtBearer(options =>
           {
               options.Authority = "https://localhost:5001";
               options.Audience = "https://localhost:5001/resources";
               // Other JWT configuration options...
           });

        // Define custom authorization policies
        service.AddFunctionsAuthorization(options =>
        {
            options.AddPolicy("Admin", policy => policy.RequireRole("super_admin"));
        });

        #endregion
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