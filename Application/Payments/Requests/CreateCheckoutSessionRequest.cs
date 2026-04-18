using Application.Payments.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Payments.Requests
{
    public record CreateCheckoutSessionRequest(
        Guid UserId,
        string SuccessUrl,
        string CancelUrl,
        string Currency = "eur")
        : ICommand<Result<CreateCheckoutSessionResponse>>;
}
