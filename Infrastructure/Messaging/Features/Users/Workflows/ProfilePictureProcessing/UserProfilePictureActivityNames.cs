namespace Infrastructure.Messaging.Features.Users.Workflows.ProfilePictureProcessing
{
    internal static class UserProfilePictureActivityNames
    {
        internal const string GeneratePictureWorkflowPaths = "GenerateUserPictureWorkflowPaths";
        internal const string DownloadFile = "DownloadUserProfilePicture";
        internal const string ResizeSmall = "ResizeUserProfilePictureSmall";
        internal const string ResizeMedium = "ResizeUserProfilePictureMedium";
        internal const string ResizeLarge = "ResizeUserProfilePictureLarge";
        internal const string UploadSmall = "UploadUserProfilePictureSmall";
        internal const string UploadMedium = "UploadUserProfilePictureMedium";
        internal const string UploadLarge = "UploadUserProfilePictureLarge";
        internal const string SynchronizeUserProfilePictures = "SynchronizeUserProfilePictures";
    }
}
