using Application.Abstractions.Persistence;
using Domain.ProcessExecutions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class ProcessTrackingStore(IDatabase database)
            : IProcessTrackingStore
    {
        public async Task<Guid> GetOrCreateProcessAsync(
            string processName,
            Guid? correlationId,
            Guid? conversationId,
            Guid? messageId,
            CancellationToken cancellationToken)
        {
            ProcessExecution? existingProcess = null;

            if (correlationId.HasValue)
            {
                existingProcess = await database.ProcessExecutions
                    .OrderByDescending(process => process.StartedDateTime)
                    .FirstOrDefaultAsync(
                        process => process.CorrelationId == correlationId && process.ProcessName == processName,
                        cancellationToken);
            }

            if (existingProcess is not null)
            {
                if (existingProcess.Status is ProcessExecutionStatus.Failed or ProcessExecutionStatus.Compensated)
                {
                    existingProcess.Status = ProcessExecutionStatus.Running;
                    existingProcess.StartedDateTime = DateTime.UtcNow;
                    existingProcess.FinishedDateTime = null;
                    existingProcess.ErrorMessage = null;

                    await database.SaveChangesAsync(cancellationToken);
                }

                return existingProcess.Id;
            }

            var newProcess = new ProcessExecution
            {
                ProcessName = processName,
                CorrelationId = correlationId,
                ConversationId = conversationId,
                MessageId = messageId,
                Status = ProcessExecutionStatus.Running,
                StartedDateTime = DateTime.UtcNow,
            };

            await database.ProcessExecutions.AddAsync(newProcess, cancellationToken);
            await database.SaveChangesAsync(cancellationToken);
            return newProcess.Id;
        }

        public async Task<Guid> StartStepAsync(
            Guid processExecutionId,
            string stepName,
            ProcessStepComponentType componentType,
            CancellationToken cancellationToken)
        {
            var attempt = (await database.ProcessStepExecutions
                    .Where(step => step.ProcessExecutionId == processExecutionId && step.StepName == stepName)
                    .Select(step => (int?)step.Attempt)
                    .MaxAsync(cancellationToken) ?? 0) + 1;

            var step = new ProcessStepExecution
            {
                ProcessExecutionId = processExecutionId,
                StepName = stepName,
                ComponentType = componentType,
                Attempt = attempt,
                Status = ProcessExecutionStatus.Running,
                StartedDateTime = DateTime.UtcNow,
            };

            await database.ProcessStepExecutions.AddAsync(step, cancellationToken);
            await database.SaveChangesAsync(cancellationToken);
            return step.Id;
        }

        public async Task CompleteStepAsync(Guid processExecutionId, Guid stepExecutionId, CancellationToken cancellationToken)
        {
            var step = await database.ProcessStepExecutions
                .SingleAsync(currentStep => currentStep.Id == stepExecutionId, cancellationToken);

            step.Status = ProcessExecutionStatus.Succeeded;
            step.FinishedDateTime = DateTime.UtcNow;

            await database.SaveChangesAsync(cancellationToken);
        }

        public async Task CompensateStepAsync(Guid processExecutionId, string stepName, CancellationToken cancellationToken)
        {
            var step = await database.ProcessStepExecutions
                .Where(currentStep => currentStep.ProcessExecutionId == processExecutionId
                                      && currentStep.StepName == stepName
                                      && currentStep.Status == ProcessExecutionStatus.Succeeded)
                .OrderByDescending(currentStep => currentStep.StartedDateTime)
                .ThenByDescending(currentStep => currentStep.Attempt)
                .FirstOrDefaultAsync(cancellationToken);

            if (step is null)
            {
                return;
            }

            step.Status = ProcessExecutionStatus.Compensated;
            step.FinishedDateTime = DateTime.UtcNow;

            var hasRunningOrSucceededSteps = await database.ProcessStepExecutions
                .AnyAsync(
                    currentStep => currentStep.ProcessExecutionId == processExecutionId
                                   && (currentStep.Status == ProcessExecutionStatus.Running
                                       || currentStep.Status == ProcessExecutionStatus.Succeeded),
                    cancellationToken);

            if (!hasRunningOrSucceededSteps)
            {
                var process = await database.ProcessExecutions
                    .SingleAsync(currentProcess => currentProcess.Id == processExecutionId, cancellationToken);

                if (process.Status != ProcessExecutionStatus.Failed)
                {
                    process.Status = ProcessExecutionStatus.Compensated;
                    process.FinishedDateTime = DateTime.UtcNow;
                }
            }

            await database.SaveChangesAsync(cancellationToken);
        }

        public async Task CompleteProcessAsync(Guid processExecutionId, CancellationToken cancellationToken)
        {
            var process = await database.ProcessExecutions
                .SingleAsync(currentProcess => currentProcess.Id == processExecutionId, cancellationToken);

            process.Status = ProcessExecutionStatus.Succeeded;
            process.FinishedDateTime = DateTime.UtcNow;
            process.ErrorMessage = string.Empty;

            await database.SaveChangesAsync(cancellationToken);
        }

        public async Task FailStepAsync(Guid processExecutionId, Guid stepExecutionId, string errorMessage, CancellationToken cancellationToken)
        {
            var step = await database.ProcessStepExecutions
                .SingleAsync(currentStep => currentStep.Id == stepExecutionId, cancellationToken);

            step.Status = ProcessExecutionStatus.Failed;
            step.FinishedDateTime = DateTime.UtcNow;
            step.ErrorMessage = errorMessage;

            var process = await database.ProcessExecutions
                .SingleAsync(currentProcess => currentProcess.Id == processExecutionId, cancellationToken);

            process.Status = ProcessExecutionStatus.Failed;
            process.FinishedDateTime = DateTime.UtcNow;
            process.ErrorMessage = errorMessage;

            await database.SaveChangesAsync(cancellationToken);
        }
    }
}