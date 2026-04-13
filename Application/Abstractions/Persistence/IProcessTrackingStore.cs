using Domain.ProcessExecutions;

namespace Application.Abstractions.Persistence
{
    public interface IProcessTrackingStore
    {
        Task<Guid> GetOrCreateProcessAsync(
            string processName,
            Guid? correlationId,
            Guid? conversationId,
            Guid? messageId,
            CancellationToken cancellationToken);

        Task<Guid> StartStepAsync(
            Guid processExecutionId,
            string stepName,
            ProcessStepComponentType componentType,
            CancellationToken cancellationToken);

        Task CompleteStepAsync(Guid processExecutionId, Guid stepExecutionId, CancellationToken cancellationToken);

        Task CompensateStepAsync(Guid processExecutionId, string stepName, CancellationToken cancellationToken);

        Task CompleteProcessAsync(Guid processExecutionId, CancellationToken cancellationToken);

        Task FailStepAsync(Guid processExecutionId, Guid stepExecutionId, string errorMessage, CancellationToken cancellationToken);
    }
}