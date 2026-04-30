namespace Infrastructure.Messaging.Features.Achievements.Workflows.AchievementPictureProcessing.Variables
{
    internal static class AchievementPictureRoutingSlipVariableNames
    {
        internal static class Picture
        {
            internal const string OriginalFilePath = "AchievementPicture.OriginalFilePath";
            internal const string SmallResizedFilePath = "AchievementPicture.SmallResizedFilePath";
            internal const string MediumResizedFilePath = "AchievementPicture.MediumResizedFilePath";
            internal const string LargeResizedFilePath = "AchievementPicture.LargeResizedFilePath";
        }

        internal static class Workflow
        {
            internal const string TemporalDirectory = "AchievementPicture.Workflow.TemporalDirectory";
        }
    }
}
