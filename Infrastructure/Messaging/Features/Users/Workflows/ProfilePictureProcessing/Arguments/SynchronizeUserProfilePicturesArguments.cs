namespace Infrastructure.Messaging.Features.Users.Workflows.ProfilePictureProcessing.Arguments
{
    public record SynchronizeUserProfilePicturesArguments(
        Guid PictureId,
        string SmallPictureVariable,
        string MediumPictureVariable,
        string LargePictureVariable,
        string SmallRelativePath,
        string MediumRelativePath,
        string LargeRelativePath);
}
