using Ardalis.Result;
using Mediator;

namespace Application.Games.Commands
{
    public record RemoveGameCommand(Guid Id)
        : ICommand<Result>;
}