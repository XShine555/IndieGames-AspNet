using Application.Payments.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Payments.Requests
{
    public record RefundFailedItemsRequest(
        Guid OrderId,
        string PaymentIntentId)
        : ICommand<Result<RefundFailedItemsResponse>>;
}
