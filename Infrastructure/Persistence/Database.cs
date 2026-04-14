using Application.Abstractions.Persistence;
using Domain.Entities;
using Domain.JobTracking;
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

        public DbSet<GameGenre> GameGenres => Set<GameGenre>();

        public DbSet<UserOwnedGame> UserOwnedGames => Set<UserOwnedGame>();

        public DbSet<UserGameCollection> UserGameCollections => Set<UserGameCollection>();

        public DbSet<UserGameCollectionItem> UserGameCollectionItems => Set<UserGameCollectionItem>();

        public DbSet<GameStorePictures> GamePictures => Set<GameStorePictures>();

        public DbSet<UserProfilePictures> UserProfilePictures => Set<UserProfilePictures>();

        public DbSet<GameArtwork> GameArtworks => Set<GameArtwork>();

        public DbSet<JobTracking> JobTrackings => Set<JobTracking>();

        public DbSet<JobTrackingStep> JobTrackingSteps => Set<JobTrackingStep>();

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

            modelBuilder.Entity<UserOwnedGame>(ownedGame =>
            {
                ownedGame.HasOne(x => x.User)
                    .WithMany(x => x.OwnedGames)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                ownedGame.HasOne(x => x.Game)
                    .WithMany(x => x.UserOwnedGames)
                    .HasForeignKey(x => x.GameId)
                    .OnDelete(DeleteBehavior.Cascade);
            } );

            modelBuilder.Entity<UserGameCollection>(collection =>
            {
                collection.HasOne(x => x.User)
                    .WithMany(x => x.GamesCollections)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            } );

            modelBuilder.Entity<UserGameCollectionItem>(collectionItem =>
            {
                collectionItem.HasOne(x => x.Collection)
                    .WithMany(x => x.Items)
                    .HasForeignKey(x => x.CollectionId)
                    .OnDelete(DeleteBehavior.Cascade);

                collectionItem.HasOne(x => x.Game)
                    .WithMany(x => x.CollectionItems)
                    .HasForeignKey(x => x.GameId)
                    .OnDelete(DeleteBehavior.Cascade);
            } );

            modelBuilder.Entity<Game>(g =>
            {
                g.HasMany(x => x.Genres)
                    .WithMany(x => x.Games)
                    .UsingEntity<GameGenre>(
                        x => x.HasOne(y => y.Genre)
                            .WithMany(y => y.GameGenres)
                            .HasForeignKey(y => y.GenreId),
                        x => x.HasOne(y => y.Game)
                            .WithMany(y => y.GameGenres)
                            .HasForeignKey(y => y.GameId),
                        x =>
                        {
                            x.ToTable("Game_Genres");
                            x.HasKey(y => new { y.GameId, y.GenreId });
                            x.Property(y => y.AddedAt).IsRequired();
                        });
            });

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
