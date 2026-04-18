namespace Application.Payments.Responses
{
    public record RefundFailedItemsResponse(
        Guid OrderId,
        string RefundId,
        decimal Amount);
}
