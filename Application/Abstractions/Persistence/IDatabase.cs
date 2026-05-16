using Domain.Entities;
using Achievement = Domain.Entities.Achievement;
using Domain.Games.Entities;
using Domain.JobTracking;
using Microsoft.EntityFrameworkCore;

namespace Application.Abstractions.Persistence
{
    public interface IDatabase
    {
        DbSet<User> Users { get; }

        DbSet<Game> Games { get; }

        DbSet<Genre> Genres { get; }

        DbSet<UserOwnedGame> UserLibrary { get; }

        DbSet<UserCartItem> UserCartItems { get; }

        DbSet<UserGameCollection> UserGameCollections { get; }

        DbSet<UserGameCollectionItem> UserGameCollectionItems { get; }

        DbSet<GameStorePictures> GamePictures { get; }

        DbSet<UserProfilePicture> UserProfilePictures { get; }

        DbSet<GameArtwork> GameArtworks { get; }

        DbSet<JobTracking> JobTrackings { get; }

        DbSet<JobTrackingStep> JobTrackingSteps { get; }

        DbSet<GameBuild> GameBuilds { get; }

        DbSet<GameBuildFile> GameBuildFiles { get; }

        DbSet<Achievement> Achievements { get; }

        DbSet<AchievementPicture> AchievementPictures { get; }

        DbSet<UserAchievement> UserAchievements { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}

