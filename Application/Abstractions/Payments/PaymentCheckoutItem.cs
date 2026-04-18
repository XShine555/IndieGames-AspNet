namespace Application.Abstractions.Payments
{
    public record PaymentCheckoutItem(
        Guid GameId,
        string Name,
        decimal UnitPrice,
        int Quantity);
}
