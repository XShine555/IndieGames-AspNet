using Application.Abstractions.Persistence;
using Domain.JobTracking;
using Infrastructure.Messaging.Features.Common.Workflows;
using Infrastructure.Messaging.Features.Games.Workflows.ArtworkProcessing.Variables;
using Infrastructure.Messaging.Features.Games.Workflows.StorePictureProcessing.Arguments;
using Infrastructure.Messaging.Features.Games.Workflows.StorePictureProcessing.Variables;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging.Features.Games.Activities
{
    public class GeneratePictureWorkflowPathsActivity(
        IProcessTrackingStore processTrackingStore,
        ILogger<GeneratePictureWorkflowPathsActivity> logger)
        : IExecuteActivity<GeneratePictureWorkflowPathsArguments>
    {
        public const string ExecuteEndpointName = "generate-picture-workflow-paths";

        internal const string ResizedPictureFileExtension = ".webp";

        public async Task<ExecutionResult> Execute(ExecuteContext<GeneratePictureWorkflowPathsArguments> executeContext)
        {
            var processExecutionId = GetProcessExecutionId(executeContext);
            var stepExecutionId = await processTrackingStore.StartStepAsync(
                processExecutionId,
                ExecuteEndpointName,
                JobTrackingType.Activity,
                executeContext.CancellationToken);

            try
            {
                var workflowPaths = CreateWorkflowPaths(executeContext.Arguments.TemporaryDirectory, executeContext.Arguments.SourceKey);

                Directory.CreateDirectory(workflowPaths.WorkingDirectory);
                logger.LogDebug("Generated picture workflow paths in {WorkingDirectory}", workflowPaths.WorkingDirectory);
                logger.LogInformation(
                    "Generate picture workflow paths activity completed for source {SourceKey} in {WorkingDirectory}",
                    executeContext.Arguments.SourceKey,
                    workflowPaths.WorkingDirectory);

                var result = executeContext.CompletedWithVariables(new Dictionary<string, object>
                {
                    [GameStorePictureRoutingSlipVariableNames.Workflow.TemporalDirectory] = workflowPaths.WorkingDirectory,
                    [GameStorePictureRoutingSlipVariableNames.Picture.OriginalFilePath] = workflowPaths.SourceFilePath,
                    [GameStorePictureRoutingSlipVariableNames.Picture.SmallResizedFilePath] = workflowPaths.SmallPictureFilePath,
                    [GameStorePictureRoutingSlipVariableNames.Picture.MediumResizedFilePath] = workflowPaths.MediumPictureFilePath,
                    [GameStorePictureRoutingSlipVariableNames.Picture.LargeResizedFilePath] = workflowPaths.LargePictureFilePath,
                    [GameStorePictureRoutingSlipVariableNames.Picture.DestinationFolderName] = workflowPaths.DestinationFolderName,
                    [GameArtworkRoutingSlipVariableNames.Picture.OriginalFilePath] = workflowPaths.SourceFilePath,
                    [GameArtworkRoutingSlipVariableNames.Picture.SmallResizedFilePath] = workflowPaths.SmallPictureFilePath,
                    [GameArtworkRoutingSlipVariableNames.Picture.MediumResizedFilePath] = workflowPaths.MediumPictureFilePath,
                    [GameArtworkRoutingSlipVariableNames.Picture.LargeResizedFilePath] = workflowPaths.LargePictureFilePath
                } );

                await processTrackingStore.CompleteStepAsync(processExecutionId, stepExecutionId, executeContext.CancellationToken);
                return result;
            }
            catch (Exception exception)
            {
                await processTrackingStore.FailStepAsync(processExecutionId, stepExecutionId, exception.Message, executeContext.CancellationToken);
                logger.LogError(exception, "Failed to generate picture workflow paths for {SourceKey}", executeContext.Arguments.SourceKey);
                throw;
            }
        }

        private static Guid GetProcessExecutionId(ExecuteContext<GeneratePictureWorkflowPathsArguments> executeContext)
        {
            var processExecutionIdValue = executeContext.GetVariable<string>(ProcessTrackingRoutingSlipVariableNames.Workflow.ProcessExecutionId)
                ?? throw new InvalidOperationException("Process execution id is required.");
            return Guid.Parse(processExecutionIdValue);
        }

        private static PictureWorkflowPaths CreateWorkflowPaths(string temporaryDirectory, string sourceKey)
        {
            var destinationFolderName = Guid.NewGuid().ToString();
            var workingDirectory = Path.Combine(temporaryDirectory, destinationFolderName);

            return new PictureWorkflowPaths(
                workingDirectory,
                Path.Combine(workingDirectory, Guid.NewGuid().ToString() + Path.GetExtension(sourceKey)),
                Path.Combine(workingDirectory, Guid.NewGuid().ToString() + ResizedPictureFileExtension),
                Path.Combine(workingDirectory, Guid.NewGuid().ToString() + ResizedPictureFileExtension),
                Path.Combine(workingDirectory, Guid.NewGuid().ToString() + ResizedPictureFileExtension),
                destinationFolderName);
        }

        private sealed record PictureWorkflowPaths(
            string WorkingDirectory,
            string SourceFilePath,
            string SmallPictureFilePath,
            string MediumPictureFilePath,
            string LargePictureFilePath,
            string DestinationFolderName);
    }
}

