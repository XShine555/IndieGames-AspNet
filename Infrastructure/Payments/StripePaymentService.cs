using Application.Abstractions.Payments;
using Microsoft.Extensions.Logging;
using Stripe;
using Stripe.Checkout;

namespace Infrastructure.Payments
{
    public class StripePaymentService(
        StripeConfiguration configuration,
        ILogger<StripePaymentService> logger)
        : IPaymentService
    {
        public async Task<PaymentCheckoutSession> CreateCheckoutSessionAsync(PaymentCheckoutSessionRequest request, CancellationToken cancellationToken)
        {
            Stripe.StripeConfiguration.ApiKey = configuration.SecretKey;

            var sessionService = new SessionService();
            var createOptions = new SessionCreateOptions
            {
                Mode = "payment",
                SuccessUrl = request.SuccessUrl,
                CancelUrl = request.CancelUrl,
                Metadata = request.Metadata.ToDictionary(pair => pair.Key, pair => pair.Value),
                PaymentIntentData = new SessionPaymentIntentDataOptions
                {
                    Metadata = request.Metadata.ToDictionary(pair => pair.Key, pair => pair.Value)
                },
                LineItems = request.Items
                    .Select(item => new SessionLineItemOptions
                    {
                        Quantity = item.Quantity,
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = request.Currency,
                            UnitAmountDecimal = item.UnitPrice * 100m,
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Name,
                                Metadata = new Dictionary<string, string>
                                {
                                    ["gameId"] = item.GameId.ToString("D")
                                }
                            }
                        }
                    } )
                    .ToList()
            };

            var session = await sessionService.CreateAsync(createOptions, cancellationToken: cancellationToken);

            logger.LogInformation(
                "Stripe checkout session {SessionId} created for order {OrderId}",
                session.Id,
                request.OrderId);

            return new PaymentCheckoutSession(
                session.Id,
                session.Url ?? string.Empty,
                session.PaymentIntentId);
        }

        public async Task<PaymentRefundResult> CreateRefundAsync(PaymentRefundRequest request, CancellationToken cancellationToken)
        {
            Stripe.StripeConfiguration.ApiKey = configuration.SecretKey;

            var refundService = new RefundService();
            var refundOptions = new RefundCreateOptions
            {
                PaymentIntent = request.PaymentIntentId,
                Amount = (long)decimal.Round(request.Amount * 100m, 0, MidpointRounding.AwayFromZero),
                Reason = request.Reason,
                Metadata = request.Metadata?.ToDictionary(pair => pair.Key, pair => pair.Value)
            };

            var refund = await refundService.CreateAsync(refundOptions, cancellationToken: cancellationToken);

            logger.LogInformation(
                "Stripe refund {RefundId} created for payment intent {PaymentIntentId}",
                refund.Id,
                request.PaymentIntentId);

            return new PaymentRefundResult(
                refund.Id,
                refund.Amount / 100m,
                refund.Status ?? string.Empty);
        }
    }
}
