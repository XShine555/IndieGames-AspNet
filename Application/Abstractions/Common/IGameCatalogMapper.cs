using Application.Games.Catalog.Responses;
using Domain.Entities;

namespace Application.Abstractions.Common
{
    public interface IGameCatalogMapper
    {
        ApplicationGame ToApplicationGame(Game game);

        ApplicationCreatedGameListItem ToApplicationCreatedGameListItem(Game game);

        ApplicationGameMutation ToApplicationGameMutation(Game game);

        ApplicationGameGenresMutation ToApplicationGameGenresMutation(Game game);
    }
}
