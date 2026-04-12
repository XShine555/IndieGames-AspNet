using Application.Abstractions.Messaging;
using MassTransit;

namespace Infrastructure.Messaging.EventBus
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
