using Application.Abstractions.Common;
using Application.Users.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Users.Commands
{
    public record UpdateUserProfilePictureCommand(
        Guid UserId,
        IFileData NewPicture)
        : ICommand<Result<ApplicationUser>>;
}