using System;
using System.Threading;
using System.Threading.Tasks;
using CoreRebusWebJobHandlerService.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rebus.Bus;


namespace CoreRebusWebJobHandlerService
{
    class HandlerService : IHostedService
    {
        private readonly ILogger<HandlerService> _logger;
        private readonly IApplicationLifetime _appLifetime;
        private readonly IServiceProvider _serviceProvider;
        private IBus _bus;
        private System.Timers.Timer _timer;

        public HandlerService(ILogger<HandlerService> logger, IApplicationLifetime appLifetime, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _appLifetime = appLifetime;

            //not sure if this is the best way to make use of the service provider.  I like being able to explictly stop/start rebus which requires
            //resolving IBus in StartAsync.  If IBus is injected it will start when it is resolved, essentially starting prior to the constructor being
            //called.  It is probably safer to use do serviceProvider.CreateScope and used a scoped sp.
            _serviceProvider = serviceProvider;  

            _appLifetime.ApplicationStarted.Register(OnStarted);
            _appLifetime.ApplicationStopping.Register(OnStopping);
            _appLifetime.ApplicationStopped.Register(OnStopped);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("{method} has been called", nameof(StartAsync));
            _bus = _serviceProvider.GetService<IBus>();

            //this can be done synchronously, returning task some Start returns quicker
            var envelopeCancelledSubscribeTask = _bus.Subscribe<SomethingHappenedEvent>()
                .ContinueWith(task => _logger.LogInformation("Subscribed to {messageType}", nameof(SomethingHappenedEvent)), cancellationToken);

            return Task.WhenAll(envelopeCancelledSubscribeTask)
                .ContinueWith(task => _logger.LogInformation("Rebus event subscriptions complete."), cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer.Stop();
            _timer.Dispose();

            _logger.LogInformation("{method} has been called", nameof(StopAsync));
            _bus.Advanced.Workers.SetNumberOfWorkers(0);
            _bus.Dispose();
            return Task.CompletedTask;
        }

        public void OnStarted()
        {
            //send a commmand
            _logger.LogInformation("{method} has been called", nameof(OnStarted));

            _timer = new System.Timers.Timer();
            _timer.Elapsed += (sender, args) =>
            {
                _bus.Send(new MakeSomethingHappenCommand() { Id = DateTimeOffset.Now.Ticks.ToString() }).ConfigureAwait(false).GetAwaiter().GetResult();
            };

            _timer.Interval = 10000;
            _timer.Enabled = true;
            _timer.Start();
        }

        public void OnStopping()
        {
            _logger.LogInformation("{method} has been called", nameof(OnStopping));
        }

        public void OnStopped()
        {
            _logger.LogInformation("{method} has been called", nameof(OnStopped));
        }
    }
}
