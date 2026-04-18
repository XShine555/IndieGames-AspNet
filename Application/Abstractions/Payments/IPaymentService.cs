namespace Application.Abstractions.Payments
{
    public interface IPaymentService
    {
        Task<PaymentCheckoutSession> CreateCheckoutSessionAsync(PaymentCheckoutSessionRequest request, CancellationToken cancellationToken);

        Task<PaymentRefundResult> CreateRefundAsync(PaymentRefundRequest request, CancellationToken cancellationToken);
    }
}
