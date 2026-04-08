using Ardalis.Result;
using Mediator;

namespace Application.Games.Commands
{
    public record RemoveGameCommand(int Id)
        : ICommand<Result>;
}