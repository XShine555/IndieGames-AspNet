using Domain.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Infrastructure.Persistence
{
    public class UpdateTimeStampInterceptor : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            UpdateEntities(eventData.Context);
            return result;
        }

        public override ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
        {
            UpdateEntities(eventData.Context);
            return new ValueTask<int>(result);
        }

        void UpdateEntities(DbContext? dbContext)
        {
            if (dbContext is null)
                return;

            var utcNow = DateTime.UtcNow;
            foreach (var entry in dbContext.ChangeTracker.Entries())
            {
                if (entry.Entity is IUpdatableEntity updatableEntity)
                {
                    if (entry.State == EntityState.Modified || entry.State == EntityState.Added)
                    {
                        updatableEntity.UpdatedAt = utcNow;
                    }
                }

                if (entry.State == EntityState.Added || entry.State == EntityState.Deleted)
                {
                    foreach (var reference in entry.References)
                    {
                        if (reference.TargetEntry?.Entity is IUpdatableEntity updatableEntityParent)
                        {
                            updatableEntityParent.UpdatedAt = utcNow;
                        }
                    }
                }
            }
        }
    }
}