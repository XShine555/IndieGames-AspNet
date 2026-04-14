using Domain.JobTracking;

namespace Application.Abstractions.Persistence
{
    public interface IProcessTrackingStore
    {
        Task<Guid> GetOrCreateProcessAsync(
            string jobName,
            Guid? correlationId,
            Guid? conversationId,
            Guid? messageId,
            CancellationToken cancellationToken);

        Task<Guid> StartStepAsync(
            Guid jobTrackingId,
            string stepName,
            JobTrackingType componentType,
            CancellationToken cancellationToken);

        Task CompleteStepAsync(Guid jobTrackingId, Guid stepExecutionId, CancellationToken cancellationToken);

        Task CompensateStepAsync(Guid jobTrackingId, string stepName, CancellationToken cancellationToken);

        Task CompleteProcessAsync(Guid jobTrackingId, CancellationToken cancellationToken);

        Task FailStepAsync(Guid jobTrackingId, Guid stepExecutionId, string errorMessage, CancellationToken cancellationToken);
    }
}
