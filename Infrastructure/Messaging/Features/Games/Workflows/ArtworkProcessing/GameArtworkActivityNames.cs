namespace Infrastructure.Messaging.Features.Games.Workflows.ArtworkProcessing
{
    internal static class GameArtworkActivityNames
    {
        internal const string GeneratePictureWorkflowPaths = "GenerateGameArtworkWorkflowPaths";
        internal const string DownloadFile = "DownloadGameArtwork";
        internal const string ResizeSmall = "ResizeGameArtworkSmall";
        internal const string ResizeMedium = "ResizeGameArtworkMedium";
        internal const string ResizeLarge = "ResizeGameArtworkLarge";
        internal const string UploadSmall = "UploadGameArtworkSmall";
        internal const string UploadMedium = "UploadGameArtworkMedium";
        internal const string UploadLarge = "UploadGameArtworkLarge";
        internal const string SynchronizeGameArtwork = "SynchronizeGameArtwork";
    }
}
