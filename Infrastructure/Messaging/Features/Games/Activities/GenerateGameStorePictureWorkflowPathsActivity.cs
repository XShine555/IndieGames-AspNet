using Application.Abstractions.Persistence;
using Domain.JobTracking;
using Infrastructure.Messaging.Features.Common.Workflows;
using Infrastructure.Messaging.Features.Games.Workflows.StorePictureProcessing.Arguments;
using Infrastructure.Messaging.Features.Games.Workflows.StorePictureProcessing.Variables;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging.Features.Games.Activities
{
    public class GenerateGameStorePictureWorkflowPathsActivity(
        IJobTrackingStore processTrackingStore,
        ILogger<GenerateGameStorePictureWorkflowPathsActivity> logger)
        : IExecuteActivity<GenerateGameStorePictureWorkflowPathsArguments>
    {
        public const string ExecuteEndpointName = "generate-game-store-picture-workflow-paths";

        internal const string ResizedPictureFileExtension = ".webp";

        public async Task<ExecutionResult> Execute(ExecuteContext<GenerateGameStorePictureWorkflowPathsArguments> executeContext)
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
                var destinationFolderName = Guid.NewGuid().ToString();
                var workingDirectory = Path.Combine(executeContext.Arguments.TemporaryDirectory, destinationFolderName);

                var sourceFilePath = Path.Combine(
                    workingDirectory,
                    Guid.NewGuid() + Path.GetExtension(executeContext.Arguments.SourceKey));
                var smallPictureFilePath = Path.Combine(
                    workingDirectory,
                    Guid.NewGuid() + ResizedPictureFileExtension);
                var mediumPictureFilePath = Path.Combine(
                    workingDirectory,
                    Guid.NewGuid() + ResizedPictureFileExtension);
                var largePictureFilePath = Path.Combine(
                    workingDirectory,
                    Guid.NewGuid() + ResizedPictureFileExtension);

                Directory.CreateDirectory(workingDirectory);
                logger.LogDebug("Generated game store picture workflow paths in {WorkingDirectory}", workingDirectory);
                logger.LogInformation(
                    "Generate game store picture workflow paths activity completed for source {SourceKey} in {WorkingDirectory}",
                    executeContext.Arguments.SourceKey,
                    workingDirectory);

                var result = executeContext.CompletedWithVariables(new Dictionary<string, object>
                {
                    [GameStorePictureRoutingSlipVariableNames.Workflow.TemporalDirectory] = workingDirectory,
                    [GameStorePictureRoutingSlipVariableNames.Picture.OriginalFilePath] = sourceFilePath,
                    [GameStorePictureRoutingSlipVariableNames.Picture.SmallResizedFilePath] = smallPictureFilePath,
                    [GameStorePictureRoutingSlipVariableNames.Picture.MediumResizedFilePath] = mediumPictureFilePath,
                    [GameStorePictureRoutingSlipVariableNames.Picture.LargeResizedFilePath] = largePictureFilePath,
                    [GameStorePictureRoutingSlipVariableNames.Picture.DestinationFolderName] = destinationFolderName
                });

                await processTrackingStore.CompleteStepAsync(processExecutionId, stepExecutionId, executeContext.CancellationToken);
                return result;
            }
            catch (Exception exception)
            {
                await processTrackingStore.FailStepAsync(processExecutionId, stepExecutionId, exception.Message, executeContext.CancellationToken);
                logger.LogError(exception, "Failed to generate game store picture workflow paths for {SourceKey}", executeContext.Arguments.SourceKey);
                throw;
            }
        }
    }
}

