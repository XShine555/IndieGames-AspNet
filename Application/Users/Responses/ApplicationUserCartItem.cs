namespace Application.Users.Responses
{
    public record ApplicationUserCartItem(
        ApplicationUserGame Game,
        DateTime AddedAt);
}
