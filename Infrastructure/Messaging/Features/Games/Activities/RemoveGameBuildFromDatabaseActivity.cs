using Application.Abstractions.Persistence;
using Domain.Games.Enums;
using Domain.JobTracking;
using Infrastructure.Messaging.Features.Common.Workflows;
using Infrastructure.Messaging.Features.Games.Workflows.BuildRemoval.Arguments;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging.Features.Games.Activities
{
    public class RemoveGameBuildFromDatabaseActivity(
        IJobTrackingStore processTrackingStore,
        IDatabase database,
        ILogger<RemoveGameBuildFromDatabaseActivity> logger)
        : IExecuteActivity<RemoveGameBuildFromDatabaseArguments>
    {
        public const string ExecuteEndpointName = "remove-game-build-from-database";

        public async Task<ExecutionResult> Execute(ExecuteContext<RemoveGameBuildFromDatabaseArguments> executeContext)
        {
            var processExecutionIdValue = executeContext.GetVariable<string>(ProcessTrackingRoutingSlipVariableNames.Workflow.ProcessExecutionId)
                ?? throw new InvalidOperationException("Process execution id is required.");
            var processExecutionId = Guid.Parse(processExecutionIdValue);

            var stepExecutionId = await processTrackingStore.StartStepAsync(
                processExecutionId,
                ExecuteEndpointName,
                JobTrackingType.Activity,
                executeContext.CancellationToken);

            try
            {
                var gameBuild = await database.GameBuilds
                    .SingleOrDefaultAsync(
                        currentBuild => currentBuild.Id == executeContext.Arguments.BuildId && currentBuild.GameId == executeContext.Arguments.GameId,
                        executeContext.CancellationToken);

                if (gameBuild is null)
                {
                    logger.LogWarning("Build {BuildId} was not found while removing it from database", executeContext.Arguments.BuildId);
                    await processTrackingStore.CompleteStepAsync(processExecutionId, stepExecutionId, executeContext.CancellationToken);
                    await processTrackingStore.CompleteJobAsync(processExecutionId, executeContext.CancellationToken);
                    return executeContext.Completed();
                }

                database.GameBuilds.Remove(gameBuild);
                await database.SaveChangesAsync(executeContext.CancellationToken);

                await processTrackingStore.CompleteStepAsync(processExecutionId, stepExecutionId, executeContext.CancellationToken);
                await processTrackingStore.CompleteJobAsync(processExecutionId, executeContext.CancellationToken);

                logger.LogInformation("Build {BuildId} removed from database", executeContext.Arguments.BuildId);
                return executeContext.Completed();
            }
            catch (Exception exception)
            {
                await MarkBuildAsFailedAsync(executeContext.Arguments.BuildId, executeContext.CancellationToken);
                await processTrackingStore.FailStepAsync(processExecutionId, stepExecutionId, exception.Message, executeContext.CancellationToken);

                logger.LogError(exception, "Error removing build {BuildId} from database", executeContext.Arguments.BuildId);
                throw;
            }
        }

        private async Task MarkBuildAsFailedAsync(Guid buildId, CancellationToken cancellationToken)
        {
            var gameBuild = await database.GameBuilds
                .SingleOrDefaultAsync(currentBuild => currentBuild.Id == buildId, cancellationToken);
            if (gameBuild is null)
                return;

            gameBuild.Status = GameBuildStatus.Failed;
            await database.SaveChangesAsync(cancellationToken);
        }
    }
}
