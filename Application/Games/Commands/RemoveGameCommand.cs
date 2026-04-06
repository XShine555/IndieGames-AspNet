using Ardalis.Result;
using Mediator;

namespace Application.Games.Commands
{
    public record RemoveGameCommand(long Id)
        : ICommand<Result>;
}