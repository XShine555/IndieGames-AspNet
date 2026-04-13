using Application.Abstractions.Common;
using Application.Games.Responses;
using Ardalis.Result;
using Domain.Entities;
using Mediator;

namespace Application.Games.Commands
{
    public record CreateGameCommand(
        string identityId,
        string Title,
        string Description,
        ICollection<int> Genres,
        ICollection<CreateGameArtworkInput> Artworks)
        : ICommand<Result<ApplicationGame>>
    {
        public static Game ToEntity(CreateGameCommand command, ICollection<Genre> Genres)
        {
            return new Game()
            {
                Title = command.Title,
                NormalizedTitle = command.Title.Trim().ToUpperInvariant(),
                Description = command.Description,
                OwnerId = command.identityId,
                Genres = Genres
            };
        }
    }

    public record CreateGameArtworkInput(
        GameArtworkType Type,
        IFileData FileData);
}