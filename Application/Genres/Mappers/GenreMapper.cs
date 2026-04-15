using Application.Abstractions.Common;
using Application.Genres.Responses;
using Domain.Entities;

namespace Application.Genres.Mappers
{
    public class GenreMapper : IGenreMapper
    {
        public ApplicationGenre ToApplicationGenre(Genre genre)
            => new(genre.Id, genre.Name, genre.CreatedAt, genre.UpdatedAt);

        public ApplicationGenreMutation ToApplicationGenreMutation(Genre genre)
            => new(genre.Id, genre.Name, genre.UpdatedAt);
    }
}
