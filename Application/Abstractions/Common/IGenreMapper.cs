using Application.Genres.Responses;
using Domain.Entities;
using System.Linq.Expressions;

namespace Application.Abstractions.Common
{
    public interface IGenreMapper
    {
        ApplicationGenre ToApplicationGenre(Genre genre);

        Expression<Func<Genre, ApplicationGenre>> ToApplicationGenreFunction { get; }

        ApplicationGenreMutation ToApplicationGenreMutation(Genre genre);
    }
}
