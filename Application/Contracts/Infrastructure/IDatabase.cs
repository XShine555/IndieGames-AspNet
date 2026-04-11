using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Contracts.Infrastructure
{
    public interface IDatabase
    {
        DbSet<User> Users { get; }

        DbSet<Game> Games { get; }

        DbSet<Genre> Genres { get; }

        DbSet<UserOwnedGame> UserOwnedGames { get; }

        DbSet<GameStorePictures> GamePictures { get; }

        DbSet<GameArtwork> GameArtworks { get; }

        DbSet<GameArtworkVariant> GameArtworkVariants { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}