namespace Application.Abstractions.Payments
{
    public record PaymentCheckoutSessionRequest(
        Guid OrderId,
        Guid UserId,
        string Currency,
        string SuccessUrl,
        string CancelUrl,
        IReadOnlyCollection<PaymentCheckoutItem> Items,
        IReadOnlyDictionary<string, string> Metadata);
}
