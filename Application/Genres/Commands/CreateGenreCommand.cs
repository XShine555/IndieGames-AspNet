using Application.Genres.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Genres.Commands
{
    public record CreateGenreCommand(
        string Name)
        : ICommand<Result<ApplicationGenreMutation>>;
}