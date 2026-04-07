using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Contracts.Infrastructure
{
    public interface IDatabase
    {
        DbSet<User> Users { get; }

        DbSet<Game> Games { get; }

        DbSet<Genre> Genres { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}