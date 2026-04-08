using Application.Genres.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Genres.Commands
{
    public record UpdateGenreCommand(
        int Id,
        string? Name)
        : ICommand<Result<ApplicationGenre>>;
}