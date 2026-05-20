using Application.Abstractions.Payment;
using Infrastructure.Configurations;
using Microsoft.Extensions.Logging;
using Stripe;
using Stripe.Checkout;

namespace Infrastructure.Services.Stripe
{
    public class StripeService(
        IStripeClient stripeClient,
        StripeSettings stripeConfig,
        ILogger<StripeService> logger)
        : IStripeService
    {
        public async Task<string> CreateCheckoutSessionAsync(
            Guid userId,
            IReadOnlyCollection<CartItemData> items,
            string successUrl,
            string cancelUrl,
            CancellationToken cancellationToken = default)
        {
            var lineItems = items.Select(item =>
            {
                var finalPrice = item.Discount > 0
                    ? Math.Round(item.Price * (1 - item.Discount / 100m), 2)
                    : item.Price;

                return new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = stripeConfig.Currency,
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Title,
                        },
                        UnitAmount = (long)(finalPrice * 100),
                    },
                    Quantity = 1,
                };
            }).ToList();

            var metadata = items
                .Select((item, index) => (index, item))
                .ToDictionary(x => $"game_{x.index}", x => x.item.GameId.ToString());

            var options = new SessionCreateOptions
            {
                Mode = "payment",
                ClientReferenceId = userId.ToString(),
                LineItems = lineItems,
                Metadata = metadata,
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl,
            };

            var service = new SessionService(stripeClient);
            var session = await service.CreateAsync(options, cancellationToken: cancellationToken);

            logger.LogInformation("Stripe checkout session {SessionId} created for user {UserId}", session.Id, userId);
            return session.Url;
        }

        public ParseWebhookOutcome ParseCheckoutCompletedEvent(string payload, string signature)
        {
            Event stripeEvent;
            try
            {
                stripeEvent = EventUtility.ConstructEvent(payload, signature, stripeConfig.WebhookSecret, throwOnApiVersionMismatch: false);
            }
            catch (StripeException ex)
            {
                logger.LogWarning(ex, "Stripe webhook signature validation failed");
                return new ParseWebhookOutcome(ParseWebhookStatus.InvalidSignature);
            }

            if (stripeEvent.Type != "checkout.session.completed")
                return new ParseWebhookOutcome(ParseWebhookStatus.UnhandledEventType);

            if (stripeEvent.Data.Object is not Session session)
                return new ParseWebhookOutcome(ParseWebhookStatus.UnhandledEventType);

            if (!Guid.TryParse(session.ClientReferenceId, out var userId))
            {
                logger.LogWarning("Stripe checkout.session.completed missing valid ClientReferenceId");
                return new ParseWebhookOutcome(ParseWebhookStatus.UnhandledEventType);
            }

            var gameIds = session.Metadata?
                .Where(kv => kv.Key.StartsWith("game_"))
                .Select(kv => Guid.TryParse(kv.Value, out var id) ? id : (Guid?)null)
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .ToArray() ?? [];

            return new ParseWebhookOutcome(ParseWebhookStatus.Success, new StripeWebhookResult(userId, gameIds));
        }
    }
}
