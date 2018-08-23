using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rebus.Config;
using Serilog;

namespace CoreRebusWebJobHandlerService
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var hostBuilder = new HostBuilder()
                .ConfigureHostConfiguration(builder =>
                {
                    //may consider using a host.json file with config seperate for appsettings.json
                    builder.AddEnvironmentVariables().AddCommandLine(args);
                })
                .ConfigureAppConfiguration((context, appbuilder) =>
                {
                    appbuilder.AddJsonFile("appsettings.json");
                })
                .ConfigureLogging((context, logBuilder) =>
                {
                    Log.Logger = new LoggerConfiguration()
                        .Enrich.WithRebusCorrelationId("RebusCorrelationId")
                        .WriteTo.LiterateConsole()
                        .CreateLogger();

                    logBuilder.AddSerilog();
                })
                .ConfigureServices((context, collection) =>
                    {
                        collection.AddCoreRebusfWebJobHandlerServiceDependencies(context.Configuration);

                        //increase the shutdown time-out.  Otherwise defaults to 5 seconds and throws error
                        collection.Configure<HostOptions>(options => options.ShutdownTimeout = TimeSpan.FromSeconds(30));

                        //this starts/stops the bus
                        collection.AddHostedService<HandlerService>();
                    })
                .UseConsoleLifetime();

            using (var host = hostBuilder.Build())
            using (var watcher = new WebJobsShutdownWatcher())
            {

                var logger = host.Services.GetService<ILogger<Program>>();

                try
                {
                    await host.StartAsync().ConfigureAwait(false);
                    await host.WaitForShutdownAsync(watcher.Token).ConfigureAwait(false);
                }
                catch (OperationCanceledException e)
                {
                    logger.LogError(e, "An operation cancelled exception has occurred.  This may be due to the host/StopAsync not having enough time to finish.");
                }
                catch (Exception e)
                {
                    logger.LogCritical(e, "An unexpected exception occurred while executing the hosts.  Please verify that the host is running.");
                }
                finally
                {
                    Log.CloseAndFlush();
                }

            }
        }
    }
}
