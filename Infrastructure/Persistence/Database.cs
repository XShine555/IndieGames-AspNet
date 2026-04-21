using Application.Abstractions.Persistence;
using Domain.Entities;
using Domain.Games.Entities;
using Domain.Games.Enums;
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

        public DbSet<UserOwnedGame> UserLibrary => Set<UserOwnedGame>();

        public DbSet<UserCartItem> UserCartItems => Set<UserCartItem>();

        public DbSet<UserGameCollection> UserGameCollections => Set<UserGameCollection>();

        public DbSet<UserGameCollectionItem> UserGameCollectionItems => Set<UserGameCollectionItem>();

        public DbSet<GameStorePictures> GamePictures => Set<GameStorePictures>();

        public DbSet<UserProfilePictures> UserProfilePictures => Set<UserProfilePictures>();

        public DbSet<GameArtwork> GameArtworks => Set<GameArtwork>();

        public DbSet<JobTracking> JobTrackings => Set<JobTracking>();

        public DbSet<JobTrackingStep> JobTrackingSteps => Set<JobTrackingStep>();

        public DbSet<GameBuild> GameBuilds => Set<GameBuild>();

        public DbSet<GameBuildFile> GameBuildFiles => Set<GameBuildFile>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(databaseConfiguration.ConnectionString, npgsqlOptions =>
            {
                npgsqlOptions.MapEnum<GameArtworkType>();
                npgsqlOptions.MapEnum<GameArtworkProcessingStatus>();
                npgsqlOptions.MapEnum<GamePictureProcessingStatus>();
                npgsqlOptions.MapEnum<GameBuildStatus>();
                npgsqlOptions.MapEnum<JobTrackingStatus>();
                npgsqlOptions.MapEnum<JobTrackingType>();
            } );
            optionsBuilder.AddInterceptors(saveChangesInterceptor);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresEnum<GameArtworkType>();
            modelBuilder.HasPostgresEnum<GameArtworkProcessingStatus>();
            modelBuilder.HasPostgresEnum<GamePictureProcessingStatus>();
            modelBuilder.HasPostgresEnum<GameBuildStatus>();
            modelBuilder.HasPostgresEnum<JobTrackingStatus>();
            modelBuilder.HasPostgresEnum<JobTrackingType>();

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

            modelBuilder.Entity<UserCartItem>(cartItem =>
            {
                cartItem.HasOne(x => x.User)
                    .WithMany(x => x.CartItems)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                cartItem.HasOne(x => x.Game)
                    .WithMany(x => x.CartItems)
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
                        } );
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

            modelBuilder.Entity<GameBuild>(build =>
            {
                build.HasOne(x => x.Game)
                    .WithMany(x => x.Builds)
                    .HasForeignKey(x => x.GameId)
                    .OnDelete(DeleteBehavior.Cascade);
            } );

            modelBuilder.Entity<GameBuildFile>(buildFile =>
            {
                buildFile.HasOne(x => x.GameBuild)
                    .WithMany(x => x.Files)
                    .HasForeignKey(x => x.GameBuildId)
                    .OnDelete(DeleteBehavior.Cascade);

                buildFile.HasIndex(x => x.GameBuildId);
            } );
        }
    }
}
