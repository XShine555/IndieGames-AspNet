using Application.Abstractions.Common;
using Application.Games.Responses;
using Ardalis.Result;
using Domain.Entities;
using Mediator;

namespace Application.Games.Commands
{
    public record CreateGameCommand(
        Guid IdentityId,
        string Title,
        string Description,
        decimal Price,
        ICollection<Guid> Genres,
        IFileData CapsulePicture,
        IFileData HeaderPicture,
        IFileData MainPicture)
        : ICommand<Result<ApplicationGame>>
    {
        public static Game ToEntity(CreateGameCommand command, ICollection<Genre> genres)
        {
            return new Game()
            {
                Title = command.Title,
                NormalizedTitle = command.Title.Trim().ToUpperInvariant(),
                Description = command.Description,
                Price = command.Price,
                Discount = 0m,
                OwnerId = command.IdentityId,
                StoreReadinessStatus = GameStoreReadinessStatus.NotReadyForStore,
                Genres = genres
            };
        }
    }
}