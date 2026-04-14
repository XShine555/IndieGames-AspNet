using Application.Abstractions.Common;
using Application.Genres.Responses;
using Application.Games.Responses;
using Application.Users.Responses;
using Domain.Entities;

namespace Application.Games.Mappers
{
    public class GameMapper : IGameMapper
    {
        public ApplicationGame ToApplicationGame(Game game)
        {
            return new ApplicationGame(
                game.Id,
                game.Title,
                game.Description,
                game.Price,
                game.Discount,
                game.StoreReadinessStatus == GameStoreReadinessStatus.ReadyForStore,
                game.IsPublic,
                game.IsPublished,
                ApplicationUserSummary.FromEntity(game.Owner),
                game.Genres.Select(ApplicationGenre.FromEntity).ToArray(),
                game.StorePictures.Select(ApplicationGamePicture.FromEntity).ToArray(),
                game.Artworks.Select(ApplicationGameArtwork.FromEntity).ToArray());
        }
    }
}
