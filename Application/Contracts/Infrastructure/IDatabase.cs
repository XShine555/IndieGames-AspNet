using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Contracts.Infrastructure
{
    public interface IDatabase
    {
        DbSet<User> Users { get; }

        DbSet<Game> Games { get; }

        DbSet<Genre> Genres { get; }

        DbSet<UserToGame> UsersToGames { get; }

        DbSet<UserToGameRequest> UsersToGameRequests { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}