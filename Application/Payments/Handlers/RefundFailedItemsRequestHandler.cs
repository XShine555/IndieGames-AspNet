using Application.Abstractions.Payments;
using Application.Abstractions.Persistence;
using Application.Payments.Requests;
using Application.Payments.Responses;
using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Payments.Handlers
{
    public class RefundFailedItemsRequestHandler(
        IDatabase database,
        IPaymentService paymentService,
        ILogger<RefundFailedItemsRequestHandler> logger)
        : ICommandHandler<RefundFailedItemsRequest, Result<RefundFailedItemsResponse>>
    {
        public async ValueTask<Result<RefundFailedItemsResponse>> Handle(RefundFailedItemsRequest request, CancellationToken cancellationToken)
        {
            var order = await database.Orders
                .Include(current => current.Items)
                .SingleOrDefaultAsync(current => current.Id == request.OrderId, cancellationToken);

            if (order is null)
            {
                logger.LogWarning("Order {OrderId} not found for refund", request.OrderId);
                return Result.NotFound();
            }

            var failedAmount = order.CalculateFailedAmount();
            if (failedAmount <= 0)
            {
                return Result.Success(new RefundFailedItemsResponse(order.Id, string.Empty, 0m));
            }

            var refund = await paymentService.CreateRefundAsync(
                new PaymentRefundRequest(
                    request.PaymentIntentId,
                    failedAmount,
                    order.Currency,
                    "requested_by_customer",
                    new Dictionary<string, string>
                    {
                        ["orderId"] = order.Id.ToString("D")
                    }),
                cancellationToken);

            order.MarkRefunded(refund.RefundedAmount);
            await database.SaveChangesAsync(cancellationToken);

            logger.LogInformation(
                "Partial refund created for order {OrderId}. Amount: {Amount}, RefundId: {RefundId}",
                order.Id,
                refund.RefundedAmount,
                refund.RefundId);

            return Result.Success(new RefundFailedItemsResponse(order.Id, refund.RefundId, refund.RefundedAmount));
        }
    }
}
