using Application.Abstractions.Persistence;
using Application.Abstractions.Common;
using Application.Abstractions.Messaging;
using Application.Abstractions.Messaging.Users.V1;
using Application.Abstractions.Storage;
using Application.Configuration;
using Application.Users.Commands;
using Ardalis.Result;
using Domain.Entities;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Users.Handlers
{
    public class UpdateUserProfilePictureCommandHandler(
        IDatabase database,
        IS3Service s3Service,
        IEventBus eventBus,
        IPictureService pictureService,
        UserConfiguration userConfiguration,
        ILogger<UpdateUserProfilePictureCommandHandler> logger)
        : ICommandHandler<UpdateUserProfilePictureCommand, Result>
    {
        public async ValueTask<Result> Handle(UpdateUserProfilePictureCommand command, CancellationToken cancellationToken)
        {
            var user = await database.Users
                .Include(u => u.ProfilePicture)
                .SingleOrDefaultAsync(u => u.IdentityId == command.UserId, cancellationToken);
            if (user is null)
            {
                logger.LogWarning("User with ID {UserId} not found", command.UserId);
                return Result.NotFound();
            }

            if (user.ProfilePicture is null)
            {
                logger.LogError("User with ID {UserId} has no profile picture record", command.UserId);
                return Result.Error("User profile picture record was not found");
            }

            if (!await pictureService.IsValidImageFormatAsync(command.NewPicture.FileStream, cancellationToken))
                return Result.Invalid(new ValidationError("The uploaded file is not a supported image format. Accepted formats: JPEG, PNG, WebP, GIF, BMP, TIFF."));

            var pictureName = Guid.NewGuid() + command.NewPicture.FileExtension;
            var sourceRoute = userConfiguration.Routes.GetProfilePicturesFolderPath(command.UserId);
            var sourceKey = BuildBucketKey(sourceRoute, pictureName);

            var uploadResult = await UploadOriginalPictureAsync(command, sourceKey, cancellationToken);
            if (!uploadResult.IsSuccess)
                return Result.Error("Error uploading profile picture");

            var previousPictureState = CaptureSnapshot(user.ProfilePicture);
            var profilePicture = UpdateOriginalProfilePicture(user.ProfilePicture, pictureName, sourceRoute, command.NewPicture.FileExtension);
            try
            {
                await database.SaveChangesAsync(cancellationToken);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error saving user profile picture metadata for user {UserId}", command.UserId);
                await s3Service.RemoveFileAsync(sourceKey, cancellationToken);
                return Result.Error("Error saving profile picture");
            }

            var enqueueResult = await PublishGeneratePicturesEventAsync(command.UserId, profilePicture.Id, sourceKey, cancellationToken);
            if (!enqueueResult.IsSuccess)
            {
                await RestorePreviousStateAsync(profilePicture, previousPictureState, cancellationToken);
                await s3Service.RemoveFileAsync(sourceKey, cancellationToken);
                return Result.Error("Error scheduling profile picture processing");
            }

            return Result.Success();
        }

        private async Task<Result> UploadOriginalPictureAsync(UpdateUserProfilePictureCommand command, string pictureKey, CancellationToken cancellationToken)
        {
            try
            {
                await s3Service.UploadFileAsync(command.NewPicture, pictureKey, cancellationToken);
                return Result.Success();
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error uploading profile picture for user with id {UserId}", command.UserId);
                return Result.Error();
            }
        }

        private static UserProfilePicture UpdateOriginalProfilePicture(UserProfilePicture profilePicture, string pictureName, string originalRelativePath, string fileExtension)
        {
            profilePicture.OriginalName = pictureName;
            profilePicture.OriginalRelativePath = originalRelativePath;
            profilePicture.OriginalFileExtension = fileExtension;
            profilePicture.AddedAt = DateTime.UtcNow;

            return profilePicture;
        }

        private async Task<Result> PublishGeneratePicturesEventAsync(Guid userId, Guid pictureId, string sourceKey, CancellationToken cancellationToken)
        {
            var @event = new GenerateUsersProfilePicturesEvent(
                pictureId,
                sourceKey,
                userConfiguration.Routes.GetSmallProfilePicturesFolderPath(userId),
                userConfiguration.Routes.GetMediumProfilePicturesFolderPath(userId),
                userConfiguration.Routes.GetLargeProfilePicturesFolderPath(userId),
                new PictureResizeSize(userConfiguration.Sizes.Small.Width, userConfiguration.Sizes.Small.Height),
                new PictureResizeSize(userConfiguration.Sizes.Medium.Width, userConfiguration.Sizes.Medium.Height),
                new PictureResizeSize(userConfiguration.Sizes.Large.Width, userConfiguration.Sizes.Large.Height));

            try
            {
                await eventBus.PublishAsync(@event, cancellationToken);
                return Result.Success();
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error publishing user profile picture generation event for picture id {PictureId}", pictureId);
                return Result.Error();
            }
        }

        private static string BuildBucketKey(string route, string fileName)
        {
            return $"{route}/{fileName}";
        }

        private async Task RestorePreviousStateAsync(
            UserProfilePicture profilePicture,
            UserProfilePictureSnapshot previousState,
            CancellationToken cancellationToken)
        {
            previousState.Apply(profilePicture);
            await database.SaveChangesAsync(cancellationToken);
        }

        private static UserProfilePictureSnapshot CaptureSnapshot(UserProfilePicture profilePicture)
        {
            return new UserProfilePictureSnapshot(
                profilePicture.OriginalName,
                profilePicture.OriginalRelativePath,
                profilePicture.OriginalFileExtension,
                profilePicture.AddedAt);
        }

        private sealed record UserProfilePictureSnapshot(
            string? OriginalName,
            string? OriginalRelativePath,
            string? OriginalFileExtension,
            DateTime AddedAt)
        {
            public void Apply(UserProfilePicture profilePicture)
            {
                profilePicture.OriginalName = OriginalName;
                profilePicture.OriginalRelativePath = OriginalRelativePath;
                profilePicture.OriginalFileExtension = OriginalFileExtension;
                profilePicture.AddedAt = AddedAt;
            }
        }
    }
}