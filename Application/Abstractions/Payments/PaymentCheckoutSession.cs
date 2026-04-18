namespace Application.Abstractions.Payments
{
    public record PaymentCheckoutSession(
        string SessionId,
        string Url,
        string? PaymentIntentId);
}
