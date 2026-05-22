using Application.Abstractions.Persistence;
using Application.Abstractions.Storage;
using Domain.JobTracking;
using Infrastructure.Messaging.Features.Common.Workflows;
using Infrastructure.Messaging.Features.Games.Workflows.StorePictureProcessing.Arguments;
using Infrastructure.Messaging.Features.Games.Workflows.StorePictureProcessing.Logs;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;

namespace Infrastructure.Messaging.Features.Common.Activities.Pictures
{
    public class ResizePictureActivity(
        IJobTrackingStore processTrackingStore,
        IPictureService pictureService,
        IDatabase database,
        ILogger<ResizePictureActivity> logger)
        : IActivity<ResizePictureLocalArguments, ResizePictureLog>
    {
        public const string ExecuteEndpointName = "resize-picture";

        public async Task<CompensationResult> Compensate(CompensateContext<ResizePictureLog> compensateContext)
        {
            try
            {
                var processExecutionIdValue = compensateContext.GetVariable<string>(ProcessTrackingRoutingSlipVariableNames.Workflow.ProcessExecutionId)
                    ?? throw new InvalidOperationException("Process execution id is required.");
                var processExecutionId = Guid.Parse(processExecutionIdValue);

                await processTrackingStore.CompensateStepAsync(
                    processExecutionId,
                    ExecuteEndpointName,
                    compensateContext.CancellationToken);

                File.Delete(compensateContext.Log.DestinationFilePath);
                return compensateContext.Compensated();
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Failed to compensate resized file {DestinationFilePath}", compensateContext.Log.DestinationFilePath);
                return compensateContext.Failed(exception);
            }
        }

        public async Task<ExecutionResult> Execute(ExecuteContext<ResizePictureLocalArguments> executeContext)
        {
            var processExecutionIdValue = executeContext.GetVariable<string>(ProcessTrackingRoutingSlipVariableNames.Workflow.ProcessExecutionId)
                ?? throw new InvalidOperationException("Process execution id is required.");
            var processExecutionId = Guid.Parse(processExecutionIdValue);

            var stepExecutionId = await processTrackingStore.StartStepAsync(
                processExecutionId,
                ExecuteEndpointName,
                JobTrackingType.Activity,
                executeContext.CancellationToken);

            var sourceFilePath = executeContext.GetVariable<string>(executeContext.Arguments.SourceFilePathVariable);
            ArgumentNullException.ThrowIfNull(sourceFilePath, nameof(sourceFilePath));
            var destinationFilePath = executeContext.GetVariable<string>(executeContext.Arguments.DestinationFilePathVariable);
            ArgumentNullException.ThrowIfNull(destinationFilePath, nameof(destinationFilePath));

            try
            {
                if (!File.Exists(sourceFilePath))
                {
                    logger.LogWarning("Source file not found: {SourceFilePath}", sourceFilePath);
                    throw new FileNotFoundException($"Source file not found: {sourceFilePath}");
                }
                await using var fileStream = File.OpenRead(sourceFilePath);
                var resizedPicture = await pictureService.ResizePictureAsWebpAsync(
                    fileStream,
                    new Size(executeContext.Arguments.Width, executeContext.Arguments.Height),
                    executeContext.CancellationToken);

                var destinationDirectory = Path.GetDirectoryName(destinationFilePath);
                if (!string.IsNullOrEmpty(destinationDirectory))
                    Directory.CreateDirectory(destinationDirectory);

                await using var destinationStream = File.Create(destinationFilePath);
                await resizedPicture.CopyToAsync(destinationStream, executeContext.CancellationToken);
                logger.LogDebug("Resized picture from {SourceFilePath} to {DestinationFilePath}",
                    sourceFilePath,
                    destinationFilePath);
                logger.LogInformation("Resize activity completed for {SourceFilePath} into {DestinationFilePath}",
                    sourceFilePath,
                    destinationFilePath);

                var result = executeContext.Completed(new ResizePictureLog(destinationFilePath));
                await processTrackingStore.CompleteStepAsync(processExecutionId, stepExecutionId, executeContext.CancellationToken);
                return result;
            }
            catch (Exception exception)
            {
                await processTrackingStore.FailStepAsync(processExecutionId, stepExecutionId, exception.Message, executeContext.CancellationToken);
                logger.LogError(exception, "An error occurred while resizing the picture from {SourceFilePath} to {DestinationFilePath} with width {Width} and height {Height}.",
                    sourceFilePath, destinationFilePath, executeContext.Arguments.Width, executeContext.Arguments.Height);

                await TryMarkEntityAsFailedAsync(executeContext.Arguments.WorkflowContextType, executeContext.Arguments.EntityId, executeContext.CancellationToken);
                throw;
            }
        }

        private async Task TryMarkEntityAsFailedAsync(
            PictureProcessingWorkflowContextType contextType,
            Guid? entityId,
            CancellationToken cancellationToken)
        {
            if (contextType == PictureProcessingWorkflowContextType.None || entityId is null)
                return;

            try
            {
                switch (contextType)
                {
                    case PictureProcessingWorkflowContextType.ArtworkProcessing:
                    {
                        var artwork = await database.GameArtworks
                            .SingleOrDefaultAsync(a => a.Id == entityId.Value, cancellationToken);
                        if (artwork is null)
                            return;
                        artwork.ProcessingStatus = Domain.Entities.GameArtworkProcessingStatus.Failed;
                        if (string.IsNullOrWhiteSpace(artwork.ProcessingError))
                            artwork.ProcessingError = "An error occurred during the processing of the game artwork.";
                        artwork.UpdatedAt = DateTime.UtcNow;
                        await database.SaveChangesAsync(cancellationToken);
                        return;
                    }
                    case PictureProcessingWorkflowContextType.StorePictureProcessing:
                    {
                        var picture = await database.GamePictures
                            .SingleOrDefaultAsync(p => p.Id == entityId.Value, cancellationToken);
                        if (picture is null)
                            return;
                        picture.ProcessingStatus = Domain.Entities.GamePictureProcessingStatus.Failed;
                        await database.SaveChangesAsync(cancellationToken);
                        return;
                    }
                    default:
                        return;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to mark entity as failed for context {ContextType} and id {EntityId}", contextType, entityId);
            }
        }
    }
}

