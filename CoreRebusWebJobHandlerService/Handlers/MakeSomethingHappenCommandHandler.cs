using System.Threading.Tasks;
using CoreRebusWebJobHandlerService.Contracts;
using Microsoft.Extensions.Logging;
using Rebus.Bus;
using Rebus.Handlers;

namespace CoreRebusWebJobHandlerService.Handlers
{
    class MakeSomethingHappenCommandHandler : IHandleMessages<MakeSomethingHappenCommand>
    {
        private readonly ILogger<MakeSomethingHappenCommandHandler> _logger;
        private readonly IBus _bus;

        public MakeSomethingHappenCommandHandler(ILogger<MakeSomethingHappenCommandHandler> logger, IBus bus)
        {
            _logger = logger;
            _bus = bus;
        }

        public async Task Handle(MakeSomethingHappenCommand message)
        {
            _logger.LogInformation("Handled {message} with Id: {id}", nameof(MakeSomethingHappenCommand), message.Id);
            await _bus.Publish(new SomethingHappenedEvent() {Id = message.Id}).ConfigureAwait(false);
        }
    }
}
