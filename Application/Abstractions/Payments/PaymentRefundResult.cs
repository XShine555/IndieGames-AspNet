namespace Application.Abstractions.Payments
{
    public record PaymentRefundResult(
        string RefundId,
        decimal RefundedAmount,
        string Status);
}
