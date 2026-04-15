namespace Application.Users.Responses
{
    public record ApplicationUserOwnedGame(
        Guid UserId,
        Guid GameId,
        DateTime PurchasedAt);
}
