using Domain.Entities;

namespace Application.Genres.Responses
{
    public record ApplicationGenre(
        int Id,
        string Name)
    {
        public static ApplicationGenre FromEntity(Genre genre)
        {
            return new ApplicationGenre(
                genre.Id,
                genre.Name);
        }
    }
}