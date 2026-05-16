using Ardalis.Result;
using Mediator;

namespace Application.Users.Commands
{
    public record CreateStripeCheckoutSessionCommand(
        Guid UserId,
        string SuccessUrl,
        string CancelUrl)
        : ICommand<Result<string>>;
}
