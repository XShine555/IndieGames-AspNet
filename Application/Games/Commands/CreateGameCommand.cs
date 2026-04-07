using Application.Games.Responses;
using Ardalis.Result;
using Domain.Entities;
using Mediator;

namespace Application.Games.Commands
{
    public record CreateGameCommand(
        Guid UserId,
        string Title,
        string Description)
        : ICommand<Result<ApplicationGame>>
    {
        public static Game ToEntity(CreateGameCommand command)
        {
            return new Game()
            {
                Title = command.Title,
                NormalizedTitle = command.Title.Trim().ToUpperInvariant(),
                Description = command.Description,
                OwnerId = command.UserId,
            };
        }
    }
}