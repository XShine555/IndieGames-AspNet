namespace Infrastructure.Messaging.Features.Games.Workflows.StorePictureProcessing
{
    internal static class GameStorePictureActivityNames
    {
        internal const string GeneratePictureWorkflowPaths = "GenerateGameStorePictureWorkflowPaths";
        internal const string DownloadFile = "DownloadGameStorePicture";
        internal const string ResizeSmall = "ResizeGameStorePictureSmall";
        internal const string ResizeMedium = "ResizeGameStorePictureMedium";
        internal const string ResizeLarge = "ResizeGameStorePictureLarge";
        internal const string UploadSmall = "UploadGameStorePictureSmall";
        internal const string UploadMedium = "UploadGameStorePictureMedium";
        internal const string UploadLarge = "UploadGameStorePictureLarge";
        internal const string SynchronizeGameStorePictures = "SynchronizeGameStorePictures";
    }
}
