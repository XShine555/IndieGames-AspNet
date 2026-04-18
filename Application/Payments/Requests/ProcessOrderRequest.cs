using Application.Payments.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Payments.Requests
{
    public record ProcessOrderRequest(
        Guid OrderId,
        string PaymentIntentId)
        : ICommand<Result<ProcessOrderResponse>>;
}
