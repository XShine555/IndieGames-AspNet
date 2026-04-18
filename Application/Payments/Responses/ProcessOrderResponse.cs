using Domain.Entities;

namespace Application.Payments.Responses
{
    public record ProcessOrderResponse(
        Guid OrderId,
        int CompletedItems,
        int FailedItems,
        OrderStatus Status,
        decimal RefundedAmount);
}
