using Application.Abstractions.Common;
using Ardalis.Result;
using Mediator;

namespace Application.Users.Commands
{
    public record UpdateUserProfilePictureCommand(
        Guid UserId,
        IFileData NewPicture)
        : ICommand<Result>;
}