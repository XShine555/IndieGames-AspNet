namespace Application.Abstractions.Payment
{
    public record CartItemData(Guid GameId, string Title, decimal Price, decimal Discount);

    public record StripeWebhookResult(Guid UserId, IReadOnlyCollection<Guid> GameIds);

    public enum ParseWebhookStatus
    {
        Success,
        InvalidSignature,
        UnhandledEventType,
    }

    public record ParseWebhookOutcome(ParseWebhookStatus Status, StripeWebhookResult? Result = null);

    public interface IStripeService
    {
        Task<string> CreateCheckoutSessionAsync(
            Guid userId,
            IReadOnlyCollection<CartItemData> items,
            string successUrl,
            string cancelUrl,
            CancellationToken cancellationToken = default);

        ParseWebhookOutcome ParseCheckoutCompletedEvent(string payload, string signature);
    }
}
