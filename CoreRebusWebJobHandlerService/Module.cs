using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rebus.Config;
using Rebus.Routing.TypeBased;
using Rebus.ServiceProvider;

namespace CoreRebusWebJobHandlerService
{
    internal static class Module
    {
        public static IServiceCollection AddCoreRebusfWebJobHandlerServiceDependencies(this IServiceCollection services, IConfiguration config)
        {

            services.AutoRegisterHandlersFromAssembly(Assembly.GetExecutingAssembly().FullName);

            services.AddRebus((configurer, provider) =>
            {
                var conn = config["AzureServiceBusConfig:ConnectionString"];
                var queue = config["AzureServiceBusConfig:QueueName"];

                return configurer
                    .Transport(t => t.UseAzureServiceBus(conn, queue))
                    .Routing(r => r.TypeBased().Map<MakeSomethingHappenCommand>(queue))  //route command back to self
                    .Logging(l => l.Serilog());
            });


            return services;
        }
    }
}
