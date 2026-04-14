using Domain.Entities;
using Domain.JobTracking;
using Microsoft.EntityFrameworkCore;

namespace Application.Abstractions.Persistence
{
    public interface IDatabase
    {
        DbSet<User> Users { get; }

        DbSet<Game> Games { get; }

        DbSet<Genre> Genres { get; }

        DbSet<UserOwnedGame> UserOwnedGames { get; }

        DbSet<UserGameCollection> UserGameCollections { get; }

        DbSet<UserGameCollectionItem> UserGameCollectionItems { get; }

        DbSet<GameStorePictures> GamePictures { get; }

        DbSet<UserProfilePictures> UserProfilePictures { get; }

        DbSet<GameArtwork> GameArtworks { get; }

        DbSet<JobTracking> JobTrackings { get; }

        DbSet<JobTrackingStep> JobTrackingSteps { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}

