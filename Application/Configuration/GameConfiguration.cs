namespace Application.Configuration
{
    public class GameConfiguration
    {
        public const string SectionName = "Game";

        public GameRoutes Routes { get; set; } = new GameRoutes();
    }

    public class GameRoutes
    {
        public string ParentFolder { get; set; } = "games";

        public string StorePicturesFolderName { get; set; } = "StorePictures";

        public string GetStorePictureFolderPath(int gameId)
        {
            return $"{ParentFolder}/{gameId}/{StorePicturesFolderName}";
        }

        public string BuildStorePicturePath(int gameId, string pictureKey)
        {
            return $"{GetStorePictureFolderPath(gameId) }/{pictureKey}";
        }
    }
}