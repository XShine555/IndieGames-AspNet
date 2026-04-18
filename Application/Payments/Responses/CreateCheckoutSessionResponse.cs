namespace Application.Payments.Responses
{
    public record CreateCheckoutSessionResponse(
        Guid OrderId,
        string CheckoutSessionId,
        string CheckoutUrl,
        decimal Amount,
        string Currency);
}
