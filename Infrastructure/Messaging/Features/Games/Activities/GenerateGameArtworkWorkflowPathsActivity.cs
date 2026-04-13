using Application.Abstractions.Persistence;
using Domain.ProcessExecutions;
using Infrastructure.Messaging.Features.Common.Workflows;
using Infrastructure.Messaging.Features.Games.Workflows.ArtworkProcessing.Arguments;
using Infrastructure.Messaging.Features.Games.Workflows.ArtworkProcessing.Variables;
using Infrastructure.Messaging.Features.Games.Workflows.StorePictureProcessing.Arguments;
using Infrastructure.Messaging.Features.Games.Workflows.StorePictureProcessing.Variables;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging.Features.Games.Activities
{
    public class GenerateGameArtworkWorkflowPathsActivity(
        IProcessTrackingStore processTrackingStore,
        ILogger<GenerateGameArtworkWorkflowPathsActivity> logger)
        : IExecuteActivity<GenerateGameArtworkWorkflowPathsArguments>
    {
        public const string ExecuteEndpointName = "generate-game-artwork-workflow-paths";

        internal const string ResizedPictureFileExtension = ".webp";

        public async Task<ExecutionResult> Execute(ExecuteContext<GenerateGameArtworkWorkflowPathsArguments> executeContext)
        {
            var processExecutionIdValue = executeContext.GetVariable<string>(ProcessTrackingRoutingSlipVariableNames.Workflow.ProcessExecutionId)
                ?? throw new InvalidOperationException("Process execution id is required.");
            var processExecutionId = Guid.Parse(processExecutionIdValue);

            var stepExecutionId = await processTrackingStore.StartStepAsync(
                processExecutionId,
                ExecuteEndpointName,
                ProcessStepComponentType.Activity,
                executeContext.CancellationToken);

            try
            {
                var destinationFolderName = Guid.NewGuid().ToString();
                var workingDirectory = Path.Combine(executeContext.Arguments.TemporaryDirectory, destinationFolderName);

                var sourceFilePath = Path.Combine(
                    workingDirectory, Guid.NewGuid().ToString() + Path.GetExtension(executeContext.Arguments.SourceKey));
                var smallPictureFilePath = Path.Combine(
                    workingDirectory, Guid.NewGuid().ToString() + ResizedPictureFileExtension);
                var mediumPictureFilePath = Path.Combine(
                    workingDirectory, Guid.NewGuid().ToString() + ResizedPictureFileExtension);
                var largePictureFilePath = Path.Combine(
                    workingDirectory, Guid.NewGuid().ToString() + ResizedPictureFileExtension);

                Directory.CreateDirectory(workingDirectory);
                logger.LogDebug("Generated game artwork workflow paths in {WorkingDirectory}", workingDirectory);
                logger.LogInformation(
                    "Generate game artwork workflow paths activity completed for source {SourceKey} in {WorkingDirectory}",
                    executeContext.Arguments.SourceKey,
                    workingDirectory);

                var result = executeContext.CompletedWithVariables(new Dictionary<string, object>
                {
                    [GameStorePictureRoutingSlipVariableNames.Workflow.TemporalDirectory] = workingDirectory,
                    [GameArtworkRoutingSlipVariableNames.Picture.OriginalFilePath] = sourceFilePath,
                    [GameArtworkRoutingSlipVariableNames.Picture.SmallResizedFilePath] = smallPictureFilePath,
                    [GameArtworkRoutingSlipVariableNames.Picture.MediumResizedFilePath] = mediumPictureFilePath,
                    [GameArtworkRoutingSlipVariableNames.Picture.LargeResizedFilePath] = largePictureFilePath
                });

                await processTrackingStore.CompleteStepAsync(processExecutionId, stepExecutionId, executeContext.CancellationToken);
                return result;
            }
            catch (Exception exception)
            {
                await processTrackingStore.FailStepAsync(processExecutionId, stepExecutionId, exception.Message, executeContext.CancellationToken);
                logger.LogError(exception, "Failed to generate game artwork workflow paths for {SourceKey}", executeContext.Arguments.SourceKey);
                throw;
            }
        }
    }
}
