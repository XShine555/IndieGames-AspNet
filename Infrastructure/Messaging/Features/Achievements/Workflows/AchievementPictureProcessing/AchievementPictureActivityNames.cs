namespace Infrastructure.Messaging.Features.Achievements.Workflows.AchievementPictureProcessing
{
    internal static class AchievementPictureActivityNames
    {
        internal const string GeneratePictureWorkflowPaths = "GenerateAchievementPictureWorkflowPaths";
        internal const string DownloadFile = "DownloadAchievementPicture";
        internal const string ResizeSmall = "ResizeAchievementPictureSmall";
        internal const string ResizeMedium = "ResizeAchievementPictureMedium";
        internal const string ResizeLarge = "ResizeAchievementPictureLarge";
        internal const string UploadSmall = "UploadAchievementPictureSmall";
        internal const string UploadMedium = "UploadAchievementPictureMedium";
        internal const string UploadLarge = "UploadAchievementPictureLarge";
        internal const string SynchronizeAchievementPictures = "SynchronizeAchievementPictures";
    }
}
