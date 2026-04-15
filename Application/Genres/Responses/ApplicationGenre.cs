using Domain.Entities;

namespace Application.Genres.Responses
{
    public record ApplicationGenre(
        Guid Id,
        string Name);
}