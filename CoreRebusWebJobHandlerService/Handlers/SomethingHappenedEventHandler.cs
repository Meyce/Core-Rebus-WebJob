using System.Threading.Tasks;
using CoreRebusWebJobHandlerService.Contracts;
using Microsoft.Extensions.Logging;
using Rebus.Handlers;

namespace CoreRebusWebJobHandlerService.Handlers
{
    class SomethingHappenedEventHandler : IHandleMessages<SomethingHappenedEvent>
    {
        private readonly ILogger<SomethingHappenedEventHandler> _logger;

        public SomethingHappenedEventHandler(ILogger<SomethingHappenedEventHandler> logger)
        {
            _logger = logger;
        }
        public Task Handle(SomethingHappenedEvent message)
        {
            _logger.LogInformation("Handled {message} with Id: {id}", nameof(SomethingHappenedEvent), message.Id);
            return Task.CompletedTask;
        }
    }
}
