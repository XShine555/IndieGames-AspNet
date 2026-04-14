using Application.Abstractions.Persistence;
using Domain.JobTracking;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class ProcessTrackingStore(IDatabase database)
            : IJobTrackingStore
    {
        public async Task<Guid> GetOrCreateProcessAsync(
            string jobName,
            Guid? correlationId,
            Guid? conversationId,
            Guid? messageId,
            CancellationToken cancellationToken)
        {
            JobTracking? existingProcess = null;

            if (correlationId.HasValue)
            {
                existingProcess = await database.JobTrackings
                    .OrderByDescending(jobTracking => jobTracking.StartedDateTime)
                    .FirstOrDefaultAsync(
                        jobTracking => jobTracking.CorrelationId == correlationId && jobTracking.JobName == jobName,
                        cancellationToken);
            }

            if (existingProcess is not null)
            {
                if (existingProcess.Status is JobTrackingStatus.Failed or JobTrackingStatus.Compensated)
                {
                    existingProcess.Status = JobTrackingStatus.Running;
                    existingProcess.StartedDateTime = DateTime.UtcNow;
                    existingProcess.FinishedDateTime = null;
                    existingProcess.ErrorMessage = null;

                    await database.SaveChangesAsync(cancellationToken);
                }

                return existingProcess.Id;
            }

            var newProcess = new JobTracking
            {
                JobName = jobName,
                CorrelationId = correlationId,
                ConversationId = conversationId,
                MessageId = messageId,
                Status = JobTrackingStatus.Running,
                StartedDateTime = DateTime.UtcNow,
            };

            await database.JobTrackings.AddAsync(newProcess, cancellationToken);
            await database.SaveChangesAsync(cancellationToken);
            return newProcess.Id;
        }

        public async Task<Guid> StartStepAsync(
            Guid jobTrackingId,
            string stepName,
            JobTrackingType componentType,
            CancellationToken cancellationToken)
        {
            var attempt = (await database.JobTrackingSteps
                    .Where(step => step.JobTrackingId == jobTrackingId && step.StepName == stepName)
                    .Select(step => (int?)step.Attempt)
                    .MaxAsync(cancellationToken) ?? 0) + 1;

            var step = new JobTrackingStep
            {
                JobTrackingId = jobTrackingId,
                StepName = stepName,
                ComponentType = componentType,
                Attempt = attempt,
                Status = JobTrackingStatus.Running,
                StartedDateTime = DateTime.UtcNow,
            };

            await database.JobTrackingSteps.AddAsync(step, cancellationToken);
            await database.SaveChangesAsync(cancellationToken);
            return step.Id;
        }

        public async Task CompleteStepAsync(Guid jobTrackingId, Guid stepExecutionId, CancellationToken cancellationToken)
        {
            var step = await database.JobTrackingSteps
                .SingleAsync(currentStep => currentStep.Id == stepExecutionId, cancellationToken);

            step.Status = JobTrackingStatus.Succeeded;
            step.FinishedDateTime = DateTime.UtcNow;

            await database.SaveChangesAsync(cancellationToken);
        }

        public async Task CompensateStepAsync(Guid jobTrackingId, string stepName, CancellationToken cancellationToken)
        {
            var step = await database.JobTrackingSteps
                .Where(currentStep => currentStep.JobTrackingId == jobTrackingId
                                      && currentStep.StepName == stepName
                                      && currentStep.Status == JobTrackingStatus.Succeeded)
                .OrderByDescending(currentStep => currentStep.StartedDateTime)
                .ThenByDescending(currentStep => currentStep.Attempt)
                .FirstOrDefaultAsync(cancellationToken);

            if (step is null)
            {
                return;
            }

            step.Status = JobTrackingStatus.Compensated;
            step.FinishedDateTime = DateTime.UtcNow;

            var hasRunningOrSucceededSteps = await database.JobTrackingSteps
                .AnyAsync(
                    currentStep => currentStep.JobTrackingId == jobTrackingId
                                   && (currentStep.Status == JobTrackingStatus.Running
                                       || currentStep.Status == JobTrackingStatus.Succeeded),
                    cancellationToken);

            if (!hasRunningOrSucceededSteps)
            {
                var process = await database.JobTrackings
                    .SingleAsync(currentProcess => currentProcess.Id == jobTrackingId, cancellationToken);

                if (process.Status != JobTrackingStatus.Failed)
                {
                    process.Status = JobTrackingStatus.Compensated;
                    process.FinishedDateTime = DateTime.UtcNow;
                }
            }

            await database.SaveChangesAsync(cancellationToken);
        }

        public async Task CompleteJobAsync(Guid jobTrackingId, CancellationToken cancellationToken)
        {
            var process = await database.JobTrackings
                .SingleAsync(currentProcess => currentProcess.Id == jobTrackingId, cancellationToken);

            process.Status = JobTrackingStatus.Succeeded;
            process.FinishedDateTime = DateTime.UtcNow;
            process.ErrorMessage = null;

            await database.SaveChangesAsync(cancellationToken);
        }

        public async Task FailStepAsync(Guid jobTrackingId, Guid stepExecutionId, string errorMessage, CancellationToken cancellationToken)
        {
            var step = await database.JobTrackingSteps
                .SingleAsync(currentStep => currentStep.Id == stepExecutionId, cancellationToken);

            step.Status = JobTrackingStatus.Failed;
            step.FinishedDateTime = DateTime.UtcNow;
            step.ErrorMessage = errorMessage;

            var process = await database.JobTrackings
                .SingleAsync(currentProcess => currentProcess.Id == jobTrackingId, cancellationToken);

            process.Status = JobTrackingStatus.Failed;
            process.FinishedDateTime = DateTime.UtcNow;
            process.ErrorMessage = errorMessage;

            await database.SaveChangesAsync(cancellationToken);
        }
    }
}
