using Application.Abstractions.Payments;
using Application.Abstractions.Persistence;
using Application.Payments.Requests;
using Application.Payments.Responses;
using Ardalis.Result;
using Domain.Entities;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Payments.Handlers
{
    public class CreateCheckoutSessionRequestHandler(
        IDatabase database,
        IPaymentService paymentService,
        ILogger<CreateCheckoutSessionRequestHandler> logger)
        : ICommandHandler<CreateCheckoutSessionRequest, Result<CreateCheckoutSessionResponse>>
    {
        public async ValueTask<Result<CreateCheckoutSessionResponse>> Handle(CreateCheckoutSessionRequest request, CancellationToken cancellationToken)
        {
            var cartItems = await database.UserCartItems
                .AsNoTracking()
                .Where(item => item.UserId == request.UserId)
                .Include(item => item.Game)
                .ToArrayAsync(cancellationToken);

            if (cartItems.Length == 0)
            {
                logger.LogWarning("User {UserId} tried to checkout with an empty cart", request.UserId);
                return Result.Invalid(new ValidationError("Cart is empty"));
            }

            var order = new Order
            {
                UserId = request.UserId,
                Currency = request.Currency.ToLowerInvariant(),
                Status = OrderStatus.Pending
            };

            var paymentItems = new List<PaymentCheckoutItem>(cartItems.Length);
            foreach (var cartItem in cartItems)
            {
                var discountedPrice = decimal.Round(
                    cartItem.Game.Price * (1m - (cartItem.Game.Discount / 100m)),
                    2,
                    MidpointRounding.AwayFromZero);

                order.AddItem(cartItem.GameId, discountedPrice);

                paymentItems.Add(new PaymentCheckoutItem(
                    cartItem.GameId,
                    cartItem.Game.Title,
                    discountedPrice,
                    1));
            }

            await database.Orders.AddAsync(order, cancellationToken);

            var metadata = new Dictionary<string, string>
            {
                ["orderId"] = order.Id.ToString("D"),
                ["userId"] = request.UserId.ToString("D")
            };

            var checkoutSession = await paymentService.CreateCheckoutSessionAsync(
                new PaymentCheckoutSessionRequest(
                    order.Id,
                    request.UserId,
                    order.Currency,
                    request.SuccessUrl,
                    request.CancelUrl,
                    paymentItems,
                    metadata),
                cancellationToken);

            order.RegisterCheckoutSession(checkoutSession.SessionId, checkoutSession.PaymentIntentId);

            await database.SaveChangesAsync(cancellationToken);

            var response = new CreateCheckoutSessionResponse(
                order.Id,
                checkoutSession.SessionId,
                checkoutSession.Url,
                order.CalculateTotalAmount(),
                order.Currency);

            return Result.Success(response);
        }
    }
}
