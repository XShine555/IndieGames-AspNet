namespace Application.Configuration
{
    public class GameConfiguration
    {
        public const string SectionName = "Game";

        public GameRoutes Routes { get; set; } = new GameRoutes();

        public GamePictureSizes Sizes { get; set; } = new GamePictureSizes();
    }

    public class GameRoutes
    {
        public string ParentFolder { get; set; } = "games";

        public string StorePicturesFolderName { get; set; } = "StorePictures";

        public string SmallPicturesFolderName { get; set; } = "SmallPictures";

        public string MediumPicturesFolderName { get; set; } = "MediumPictures";

        public string LargePicturesFolderName { get; set; } = "LargePictures";

        public string GetStorePictureFolderPath(int gameId)
        {
            return $"{ParentFolder}/{gameId}/{StorePicturesFolderName}";
        }

        public string GetSmallPictureFolderPath(int gameId)
        {
            return $"{ParentFolder}/{gameId}/{SmallPicturesFolderName}";
        }

        public string GetMediumPictureFolderPath(int gameId)
        {
            return $"{ParentFolder}/{gameId}/{MediumPicturesFolderName}";
        }

        public string GetLargePictureFolderPath(int gameId)
        {
            return $"{ParentFolder}/{gameId}/{LargePicturesFolderName}";
        }

        public string BuildStorePicturePath(int gameId, string pictureKey)
        {
            return $"{GetStorePictureFolderPath(gameId) }/{pictureKey}";
        }
    }

    public class GamePictureSizes
    {
        public GamePictureSize Small { get; set; } = new GamePictureSize() { Width = 320, Height = 180 };

        public GamePictureSize Medium { get; set; } = new GamePictureSize() { Width = 640, Height = 360 };

        public GamePictureSize Large { get; set; } = new GamePictureSize() { Width = 1280, Height = 720 };
    }

    public class GamePictureSize
    {
        public int Width { get; set; }

        public int Height { get; set; }
    }
}