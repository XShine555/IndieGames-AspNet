using Application.Contracts.Infrastructure;
using Domain.Entities;
using Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class Database(DatabaseConfiguration databaseConfiguration, UpdateTimeStampInterceptor updateTimeStampInterceptor)
        : DbContext, IDatabase
    {
        public DbSet<User> Users => Set<User>();

        public DbSet<Game> Games => Set<Game>();

        public DbSet<Genre> Genres => Set<Genre>();

        public DbSet<UserOwnedGame> UserOwnedGames => Set<UserOwnedGame>();

        public DbSet<GameOriginalPicture> GamePictures => Set<GameOriginalPicture>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL(databaseConfiguration.ConnectionString);
            optionsBuilder.AddInterceptors(updateTimeStampInterceptor);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(u =>
            {
                u.HasIndex(x => x.IdentityId).IsUnique();
            } );
        }
    }
}