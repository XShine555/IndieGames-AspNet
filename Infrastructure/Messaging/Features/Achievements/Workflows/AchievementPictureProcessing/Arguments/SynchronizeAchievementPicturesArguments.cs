namespace Infrastructure.Messaging.Features.Achievements.Workflows.AchievementPictureProcessing.Arguments
{
    public record SynchronizeAchievementPicturesArguments(
        Guid PictureId,
        string SmallPictureVariable,
        string MediumPictureVariable,
        string LargePictureVariable,
        string SmallRelativePath,
        string MediumRelativePath,
        string LargeRelativePath);
}
