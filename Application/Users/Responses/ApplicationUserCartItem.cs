using Application.Games.Responses;

namespace Application.Users.Responses
{
    public record ApplicationUserCartItem(
        Guid GameId,
        ApplicationGame Game,
        DateTime AddedAt);
}
