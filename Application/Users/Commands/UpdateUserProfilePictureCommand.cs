using Application.Abstractions.Common;
using Application.Users.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Users.Commands
{
    public record UpdateUserProfilePictureCommand(
        string UserId,
        IFileData NewPicture)
        : ICommand<Result<ApplicationUser>>;
}