using Ardalis.Result;
using Mediator;

namespace Application.Genres.Commands
{
    public record RemoveGenreCommand(Guid Id)
        : ICommand<Result>;
}