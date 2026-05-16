namespace Application.Configuration
{
    public class AchievementsConfiguration
    {
        public const string SectionName = "Achievements";

        public int MaxAchievementsPerGame { get; set; } = 30;

        public AchievementRoutes Routes { get; set; } = new AchievementRoutes();

        public AchievementPictureSizes Sizes { get; set; } = new AchievementPictureSizes();
    }

    public class AchievementRoutes
    {
        public string ParentFolder { get; set; } = "games";

        public string AchievementsFolderName { get; set; } = "achievements";

        public string PicturesFolderName { get; set; } = "Pictures";

        public string SmallPicturesFolderName { get; set; } = "SmallPictures";

        public string MediumPicturesFolderName { get; set; } = "MediumPictures";

        public string LargePicturesFolderName { get; set; } = "LargePictures";

        public string GetAchievementFolderPath(Guid gameId, Guid achievementId)
        {
            return $"{ParentFolder}/{gameId}/{AchievementsFolderName}/{achievementId}";
        }

        public string GetPicturesFolderPath(Guid gameId, Guid achievementId)
        {
            return $"{GetAchievementFolderPath(gameId, achievementId) }/{PicturesFolderName}";
        }

        public string GetSmallPicturesFolderPath(Guid gameId, Guid achievementId)
        {
            return $"{GetAchievementFolderPath(gameId, achievementId)}/{SmallPicturesFolderName}";
        }

        public string GetMediumPicturesFolderPath(Guid gameId, Guid achievementId)
        {
            return $"{GetAchievementFolderPath(gameId, achievementId) }/{MediumPicturesFolderName}";
        }

        public string GetLargePicturesFolderPath(Guid gameId, Guid achievementId)
        {
            return $"{GetAchievementFolderPath(gameId, achievementId) }/{LargePicturesFolderName}";
        }

        public string BuildBucketKey(string route, string fileName)
        {
            return $"{route}/{fileName}";
        }
    }

    public class AchievementPictureSizes
    {
        public AchievementPictureSize Small { get; set; } = new AchievementPictureSize() { Width = 64, Height = 64 };

        public AchievementPictureSize Medium { get; set; } = new AchievementPictureSize() { Width = 128, Height = 128 };

        public AchievementPictureSize Large { get; set; } = new AchievementPictureSize() { Width = 256, Height = 256 };
    }

    public class AchievementPictureSize
    {
        public int Width { get; set; }

        public int Height { get; set; }
    }
}
