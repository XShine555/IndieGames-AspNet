using Domain.Entities;

namespace Application.Configuration
{
    public class GameConfiguration
    {
        public const string SectionName = "Game";

        public GameRoutes Routes { get; set; } = new GameRoutes();

        public GamePictureSizes Sizes { get; set; } = new GamePictureSizes();

        public GamePictureSizes ArtworkSizes { get; set; } = new GamePictureSizes();
    }

    public class GameRoutes
    {
        public string ParentFolder { get; set; } = "games";

        public string StorePicturesFolderName { get; set; } = "StorePictures";

        public string SmallPicturesFolderName { get; set; } = "SmallPictures";

        public string MediumPicturesFolderName { get; set; } = "MediumPictures";

        public string LargePicturesFolderName { get; set; } = "LargePictures";

        public string ArtworksFolderName { get; set; } = "Artworks";

        public string OriginalArtworkFolderName { get; set; } = "Original";

        public string SmallArtworkFolderName { get; set; } = "Small";

        public string MediumArtworkFolderName { get; set; } = "Medium";

        public string LargeArtworkFolderName { get; set; } = "Large";

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

        public string GetOriginalArtworkFolderPath(int gameId, GameArtworkType artworkType)
        {
            return BuildArtworkFolderPath(gameId, artworkType, OriginalArtworkFolderName);
        }

        public string GetSmallArtworkFolderPath(int gameId, GameArtworkType artworkType)
        {
            return BuildArtworkFolderPath(gameId, artworkType, SmallArtworkFolderName);
        }

        public string GetMediumArtworkFolderPath(int gameId, GameArtworkType artworkType)
        {
            return BuildArtworkFolderPath(gameId, artworkType, MediumArtworkFolderName);
        }

        public string GetLargeArtworkFolderPath(int gameId, GameArtworkType artworkType)
        {
            return BuildArtworkFolderPath(gameId, artworkType, LargeArtworkFolderName);
        }

        public string BuildOriginalArtworkPath(int gameId, GameArtworkType artworkType, string pictureKey)
        {
            return $"{GetOriginalArtworkFolderPath(gameId, artworkType)}/{pictureKey}";
        }

        private string BuildArtworkFolderPath(int gameId, GameArtworkType artworkType, string sizeFolder)
        {
            return $"{ParentFolder}/{gameId}/{ArtworksFolderName}/{artworkType}/{sizeFolder}";
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