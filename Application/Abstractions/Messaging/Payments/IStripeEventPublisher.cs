using Application.Abstractions.Messaging.Payments.V1;

namespace Application.Abstractions.Messaging.Payments
{
    public interface IStripeEventPublisher
    {
        Task PublishCheckoutCompletedAsync(StripeCheckoutCompletedEvent stripeEvent, CancellationToken cancellationToken);
    }
}
