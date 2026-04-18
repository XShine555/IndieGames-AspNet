using Application.Abstractions.Messaging;
using Application.Abstractions.Messaging.Payments;
using Application.Abstractions.Messaging.Payments.V1;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Payments
{
    public class StripeSqsEventPublisher(
        IEventBus eventBus,
        ILogger<StripeSqsEventPublisher> logger)
        : IStripeEventPublisher
    {
        public async Task PublishCheckoutCompletedAsync(StripeCheckoutCompletedEvent stripeEvent, CancellationToken cancellationToken)
        {
            await eventBus.PublishAsync(stripeEvent, cancellationToken);
            logger.LogInformation(
                "Stripe event {EventId} published to messaging layer for order {OrderId}",
                stripeEvent.EventId,
                stripeEvent.OrderId);
        }
    }
}
