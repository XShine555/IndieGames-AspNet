using Application.Contracts.Infrastructure;
using Domain.Entities;
using Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class Database(DatabaseConfiguration databaseConfiguration)
        : DbContext, IDatabase
    {
        public DbSet<User> Users => Set<User>();

        public DbSet<Game> Games => Set<Game>();

        public DbSet<Genre> Genres => Set<Genre>();

        public DbSet<UserOwnedGame> UserOwnedGames => Set<UserOwnedGame>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL(databaseConfiguration.ConnectionString);
        }
    }
}