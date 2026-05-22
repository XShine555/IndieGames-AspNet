using Application.Abstractions.Common;
using Application.Abstractions.Persistence;
using Application.Configuration;
using Application.Users.Commands;
using Application.Users.Responses;
using Ardalis.Result;
using Domain.Entities;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Users.Handlers
{
    public class CreateUserCommandHandler(
        IDatabase database,
        IUserMapper userMapper,
        UserConfiguration userConfiguration,
        ILogger<CreateUserCommandHandler> logger)
        : ICommandHandler<CreateUserCommand, Result<ApplicationUserMutation>>
    {
        public async ValueTask<Result<ApplicationUserMutation>> Handle(CreateUserCommand command, CancellationToken cancellationToken)
        {
            var anyUser = await database.Users.AnyAsync(u => u.IdentityId == command.IdentityId, cancellationToken);
            if (anyUser)
            {
                logger.LogWarning("User with identity id {IdentityId} already exists", command.IdentityId);
                return Result.Conflict();
            }

            var newUser = new User
            {
                IdentityId = command.IdentityId,
                Username = command.Username,
                DisplayUsername = command.Username,
                NormalizedDisplayUsername = command.Username.Trim().ToUpperInvariant(),
                ProfilePicture = new UserProfilePicture
                {
                    UserId = command.IdentityId,
                    SmallRelativePath = userConfiguration.Routes.ParentFolder,
                    SmallName = userConfiguration.Routes.PresetSmallProfilePicture,
                    SmallFileExtension = Path.GetExtension(userConfiguration.Routes.PresetSmallProfilePicture),
                    MediumRelativePath = userConfiguration.Routes.ParentFolder,
                    MediumName = userConfiguration.Routes.PresetMediumProfilePicture,
                    MediumFileExtension = Path.GetExtension(userConfiguration.Routes.PresetMediumProfilePicture),
                    LargeRelativePath = userConfiguration.Routes.ParentFolder,
                    LargeName = userConfiguration.Routes.PresetLargeProfilePicture,
                    LargeFileExtension = Path.GetExtension(userConfiguration.Routes.PresetLargeProfilePicture)
                }
            };

            await database.Users.AddAsync(newUser, cancellationToken);
            await database.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Created new user with id {UserId} and identity id {IdentityId}", newUser.IdentityId, newUser.IdentityId);

            return Result.Success(userMapper.ToApplicationUserMutation(newUser));
        }
    }
}