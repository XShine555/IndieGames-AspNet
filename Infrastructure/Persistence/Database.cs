using Application.Contracts.Infrastructure;
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

        public DbSet<GameArtwork> GameArtworks => Set<GameArtwork>();

        public DbSet<GameArtworkVariant> GameArtworkVariants => Set<GameArtworkVariant>();

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

            modelBuilder.Entity<GameArtwork>(a =>
            {
                a.HasOne(x => x.Game)
                    .WithMany(x => x.Artworks)
                    .HasForeignKey(x => x.GameId)
                    .OnDelete(DeleteBehavior.Cascade);

                a.Property(x => x.ProcessingError)
                    .HasMaxLength(512);
            } );

            modelBuilder.Entity<GameArtworkVariant>(v =>
            {
                v.HasOne(x => x.GameArtwork)
                    .WithMany(x => x.Variants)
                    .HasForeignKey(x => x.GameArtworkId)
                    .OnDelete(DeleteBehavior.Cascade);

                v.ToTable(t =>
                {
                    t.HasCheckConstraint("CK_GameArtworkVariant_Width", "Width > 0");
                    t.HasCheckConstraint("CK_GameArtworkVariant_Height", "Height > 0");
                    t.HasCheckConstraint("CK_GameArtworkVariant_FileSizeInBytes", "FileSizeInBytes > 0");
                } );
            } );
        }
    }
}