namespace Application.Games.Catalog.Responses
{
    public record ApplicationCreatedGameListItem(
        Guid Id,
        string Title,
        string Description,
        GameStatusType Status,
        string? PictureUrl);
}