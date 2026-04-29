using Application.Games.Catalog.Responses;
using Domain.Entities;
using System.Linq.Expressions;

namespace Application.Abstractions.Common
{
    public interface IGameCatalogMapper
    {
        Expression<Func<Game, ApplicationGameListItem>> ToApplicationGameListItemFunction { get; }

        ApplicationGame ToApplicationGame(Game game);

        ApplicationGameMutation ToApplicationGameMutation(Game game);

        ApplicationGameGenresMutation ToApplicationGameGenresMutation(Game game);
    }
}
