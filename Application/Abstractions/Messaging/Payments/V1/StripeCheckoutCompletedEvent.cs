namespace Application.Abstractions.Messaging.Payments.V1
{
    public record StripeCheckoutCompletedEvent(
        string EventId,
        string EventType,
        Guid OrderId,
        string CheckoutSessionId,
        string PaymentIntentId);
}
