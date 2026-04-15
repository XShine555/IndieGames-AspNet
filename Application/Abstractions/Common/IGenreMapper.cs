using Application.Genres.Responses;
using Domain.Entities;

namespace Application.Abstractions.Common
{
    public interface IGenreMapper
    {
        ApplicationGenre ToApplicationGenre(Genre genre);

        ApplicationGenreMutation ToApplicationGenreMutation(Genre genre);
    }
}
