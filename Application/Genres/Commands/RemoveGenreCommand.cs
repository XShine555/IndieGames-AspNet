using Ardalis.Result;
using Mediator;

namespace Application.Genres.Commands
{
    public record RemoveGenreCommand(int Id)
        : ICommand<Result>;
}