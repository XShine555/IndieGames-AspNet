namespace Application.Abstractions.Payments
{
    public record PaymentRefundRequest(
        string PaymentIntentId,
        decimal Amount,
        string Currency,
        string Reason,
        IReadOnlyDictionary<string, string>? Metadata = null);
}
