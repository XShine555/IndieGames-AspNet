using Application.Abstractions.Payment;
using Application.Abstractions.Persistence;
using Application.Users.Commands;
using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Users.Handlers
{
    public class CreateStripeCheckoutSessionCommandHandler(
        IDatabase database,
        IStripeService stripeService,
        ILogger<CreateStripeCheckoutSessionCommandHandler> logger)
        : ICommandHandler<CreateStripeCheckoutSessionCommand, Result<string>>
    {
        public async ValueTask<Result<string>> Handle(CreateStripeCheckoutSessionCommand command, CancellationToken cancellationToken)
        {
            var userExists = await database.Users
                .AsNoTracking()
                .AnyAsync(u => u.IdentityId == command.UserId, cancellationToken);
            if (!userExists)
            {
                logger.LogWarning("User {UserId} not found", command.UserId);
                return Result.NotFound("User not found");
            }

            var cartItems = await database.UserCartItems
                .AsNoTracking()
                .Where(ci => ci.UserId == command.UserId)
                .Include(ci => ci.Game)
                .ToArrayAsync(cancellationToken);

            if (cartItems.Length == 0)
                return Result.Invalid(new ValidationError("Cart is empty"));

            var items = cartItems
                .Select(ci => new CartItemData(ci.Game.Id, ci.Game.Title, ci.Game.Price, ci.Game.Discount))
                .ToArray();

            var sessionUrl = await stripeService.CreateCheckoutSessionAsync(
                command.UserId, items, command.SuccessUrl, command.CancelUrl, cancellationToken);

            return Result.Success(sessionUrl);
        }
    }
}
