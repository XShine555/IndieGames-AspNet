using Ardalis.Result;
using Mediator;

namespace Application.Payments.Requests
{
    public record ProcessStripeEventRequest(
        string EventId,
        string EventType,
        Guid OrderId,
        string CheckoutSessionId,
        string PaymentIntentId)
        : ICommand<Result>;
}
