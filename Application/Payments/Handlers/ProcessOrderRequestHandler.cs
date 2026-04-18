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
    public class ProcessOrderRequestHandler(
        IDatabase database,
        IMediator mediator,
        ILogger<ProcessOrderRequestHandler> logger)
        : ICommandHandler<ProcessOrderRequest, Result<ProcessOrderResponse>>
    {
        public async ValueTask<Result<ProcessOrderResponse>> Handle(ProcessOrderRequest request, CancellationToken cancellationToken)
        {
            var order = await database.Orders
                .Include(current => current.Items)
                .SingleOrDefaultAsync(current => current.Id == request.OrderId, cancellationToken);

            if (order is null)
            {
                logger.LogWarning("Order {OrderId} not found", request.OrderId);
                return Result.NotFound();
            }

            if (!string.IsNullOrWhiteSpace(request.PaymentIntentId)
                && string.IsNullOrWhiteSpace(order.StripePaymentIntentId))
            {
                order.StripePaymentIntentId = request.PaymentIntentId;
            }

            var pendingItems = order.Items
                .Where(item => item.Status == OrderItemStatus.Pending)
                .ToArray();

            if (pendingItems.Length == 0)
            {
                return Result.Success(new ProcessOrderResponse(
                    order.Id,
                    order.Items.Count(item => item.Status == OrderItemStatus.Completed),
                    order.Items.Count(item => item.Status == OrderItemStatus.Failed),
                    order.Status,
                    order.RefundedAmount));
            }

            foreach (var item in pendingItems)
            {
                try
                {
                    var alreadyOwned = await database.UserLibrary
                        .AsNoTracking()
                        .AnyAsync(current => current.UserId == order.UserId && current.GameId == item.GameId, cancellationToken);

                    if (alreadyOwned)
                    {
                        order.MarkItemFailed(item.Id, "Game already owned");
                        continue;
                    }

                    await database.UserLibrary.AddAsync(new UserOwnedGame
                    {
                        UserId = order.UserId,
                        GameId = item.GameId,
                        purchasedAt = DateTime.UtcNow
                    }, cancellationToken);

                    var userCartItem = await database.UserCartItems
                        .SingleOrDefaultAsync(current => current.UserId == order.UserId && current.GameId == item.GameId, cancellationToken);

                    if (userCartItem is not null)
                    {
                        database.UserCartItems.Remove(userCartItem);
                    }

                    order.MarkItemCompleted(item.Id);
                }
                catch (Exception exception)
                {
                    logger.LogError(exception,
                        "Failed processing order item {OrderItemId} for order {OrderId}",
                        item.Id,
                        order.Id);

                    order.MarkItemFailed(item.Id, "Unexpected processing error");
                }
            }

            await database.SaveChangesAsync(cancellationToken);

            if ((order.Status == OrderStatus.PartiallyFailed || order.Status == OrderStatus.Failed)
                && !string.IsNullOrWhiteSpace(order.StripePaymentIntentId))
            {
                await mediator.Send(new RefundFailedItemsRequest(order.Id, order.StripePaymentIntentId), cancellationToken);
            }

            var response = new ProcessOrderResponse(
                order.Id,
                order.Items.Count(item => item.Status == OrderItemStatus.Completed),
                order.Items.Count(item => item.Status == OrderItemStatus.Failed),
                order.Status,
                order.RefundedAmount);

            return Result.Success(response);
        }
    }
}
