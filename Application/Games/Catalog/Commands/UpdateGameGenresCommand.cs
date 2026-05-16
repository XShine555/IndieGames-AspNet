using Application.Games.Catalog.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Games.Catalog.Commands
{
    public record UpdateGameGenresCommand(
        Guid IdentityId,
        Guid GameId,
        ICollection<Guid> Genres)
        : ICommand<Result<ApplicationGameGenresMutation>>;
}
