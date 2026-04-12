namespace Application.Configuration
{
    public class UserConfiguration
    {
        public const string SectionName = "UserConfiguration";

        public UserRoutes Routes { get; set; } = new UserRoutes();

        public UserProfilePictureSizes Sizes { get; set; } = new UserProfilePictureSizes();
    }

    public class UserRoutes
    {
        public string ParentFolder { get; set; } = "users";

        public string ProfilePicturesFolder { get; set; } = "ProfilePictures";

        public string SmallProfilePicturesFolder { get; set; } = "SmallProfilePictures";

        public string MediumProfilePicturesFolder { get; set; } = "MediumProfilePictures";

        public string LargeProfilePicturesFolder { get; set; } = "LargeProfilePictures";

        public string GetProfilePicturesFolderPath(string identityId)
        {
            return $"{ParentFolder}/{identityId}/{ProfilePicturesFolder}";
        }

        public string GetSmallProfilePicturesFolderPath(string identityId)
        {
            return $"{ParentFolder}/{identityId}/{SmallProfilePicturesFolder}";
        }

        public string GetMediumProfilePicturesFolderPath(string identityId)
        {
            return $"{ParentFolder}/{identityId}/{MediumProfilePicturesFolder}";
        }

        public string GetLargeProfilePicturesFolderPath(string identityId)
        {
            return $"{ParentFolder}/{identityId}/{LargeProfilePicturesFolder}";
        }

        public string PresetSmallProfilePicture { get; set; } = "preset_small_profile_picture.png";

        public string PresetMediumProfilePicture { get; set; } = "preset_medium_profile_picture.png";

        public string PresetLargeProfilePicture { get; set; } = "preset_large_profile_picture.png";
    }

    public class UserProfilePictureSizes
    {
        public UserProfilePictureSize Small { get; set; } = new UserProfilePictureSize() { Width = 64, Height = 64 };

        public UserProfilePictureSize Medium { get; set; } = new UserProfilePictureSize() { Width = 128, Height = 128 };

        public UserProfilePictureSize Large { get; set; } = new UserProfilePictureSize() { Width = 256, Height = 256 };
    }

    public class UserProfilePictureSize
    {
        public int Width { get; set; }

        public int Height { get; set; }
    }
}