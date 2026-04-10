using Application.Contracts.Infrastructure;
using MassTransit;

namespace Infrastructure.MassTransit.EventBus
{
    public class MassTransitEventBus(IPublishEndpoint publishEndpoint)
        : IEventBus
    {
        public async Task PublishAsync<T>(T message, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(message, nameof(message));

            await publishEndpoint.Publish(message, cancellationToken);
        }
    }
}