namespace Application.Users.Responses
{
    public record ApplicationUserCartItem(
        Guid GameId,
        ApplicationUserGame Game,
        DateTime AddedAt);
}
