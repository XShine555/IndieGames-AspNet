using Application.Abstractions.Persistence;
using Domain.Entities;
using Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Infrastructure.Persistence
{
    public class Database(DatabaseConfiguration databaseConfiguration, SaveChangesInterceptor saveChangesInterceptor)
        : DbContext, IDatabase
    {
        public DbSet<User> Users => Set<User>();

        public DbSet<Game> Games => Set<Game>();

        public DbSet<Genre> Genres => Set<Genre>();

        public DbSet<UserOwnedGame> UserOwnedGames => Set<UserOwnedGame>();

        public DbSet<GameStorePictures> GamePictures => Set<GameStorePictures>();

        public DbSet<UserProfilePictures> UserProfilePictures => Set<UserProfilePictures>();

        public DbSet<GameArtwork> GameArtworks => Set<GameArtwork>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL(databaseConfiguration.ConnectionString);
            optionsBuilder.AddInterceptors(saveChangesInterceptor);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(u =>
            {
                u.HasIndex(x => x.IdentityId).IsUnique();
            } );

            modelBuilder.Entity<UserProfilePictures>(p =>
            {
                p.HasIndex(x => x.UserId).IsUnique();
                p.HasOne(x => x.User)
                    .WithOne(x => x.ProfilePicture)
                    .HasForeignKey<UserProfilePictures>(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            } );

            modelBuilder.Entity<GameArtwork>(a =>
            {
                a.HasOne(x => x.Game)
                    .WithMany(x => x.Artworks)
                    .HasForeignKey(x => x.GameId)
                    .OnDelete(DeleteBehavior.Cascade);

                a.Property(x => x.ProcessingError)
                    .HasMaxLength(512);
            } );

            modelBuilder.Entity<Genre>().HasData(
                CreateGenre(1, "Action"),
                CreateGenre(2, "Adventure"),
                CreateGenre(3, "RPG"),
                CreateGenre(4, "Strategy"),
                CreateGenre(5, "Simulation"),
                CreateGenre(6, "Sports"),
                CreateGenre(7, "Puzzle"),
                CreateGenre(8, "Horror"),
                CreateGenre(9, "Racing"),
                CreateGenre(10, "Indie"),
                CreateGenre(11, "FPS")
            );
        }

        Genre CreateGenre(int id, string name)
        {
            return new Genre
            {
                Id = id,
                Name = name,
                NormalizedName = name.Trim().ToUpperInvariant()
            };
        }
    }
}