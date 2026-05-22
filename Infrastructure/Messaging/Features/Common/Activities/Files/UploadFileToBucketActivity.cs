using Application.Abstractions.Persistence;
using Application.Abstractions.Storage;
using Domain.JobTracking;
using Infrastructure.Messaging.Features.Common.Workflows;
using Infrastructure.Messaging.Features.Games.Workflows.StorePictureProcessing.Arguments;
using Infrastructure.Messaging.Features.Games.Workflows.StorePictureProcessing.Logs;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MimeMapping;

namespace Infrastructure.Messaging.Features.Common.Activities.Files
{
    public class UploadFileToBucketActivity(
        IJobTrackingStore processTrackingStore,
        IS3Service s3Service,
        IDatabase database,
        ILogger<UploadFileToBucketActivity> logger)
        : IActivity<UploadFileToBucketArguments, UploadFileToBucketLog>
    {
        public const string ExecuteEndpointName = "upload-file-to-bucket";

        public async Task<ExecutionResult> Execute(ExecuteContext<UploadFileToBucketArguments> executeContext)
        {
            var processExecutionIdValue = executeContext.GetVariable<string>(ProcessTrackingRoutingSlipVariableNames.Workflow.ProcessExecutionId)
                ?? throw new InvalidOperationException("Process execution id is required.");
            var processExecutionId = Guid.Parse(processExecutionIdValue);

            var stepExecutionId = await processTrackingStore.StartStepAsync(
                processExecutionId,
                ExecuteEndpointName,
                JobTrackingType.Activity,
                executeContext.CancellationToken);

            var sourceFilePath = executeContext.GetVariable<string>(executeContext.Arguments.FilePathVariable);
            ArgumentNullException.ThrowIfNull(sourceFilePath, nameof(sourceFilePath));
            var fileName = Path.GetFileName(sourceFilePath);

            try
            {
                using var fileStream = File.OpenRead(sourceFilePath);
                await s3Service.UploadFileAsync(
                    fileStream,
                    $"{executeContext.Arguments.DestinationRoute}/{fileName}",
                    MimeUtility.GetMimeMapping(fileName),
                    executeContext.CancellationToken);

                logger.LogInformation("Upload activity completed for file {FileName} into route {DestinationRoute}",
                    fileName,
                    executeContext.Arguments.DestinationRoute);

                var result = executeContext.CompletedWithVariables(new UploadFileToBucketLog(
                    executeContext.Arguments.DestinationRoute));

                await processTrackingStore.CompleteStepAsync(processExecutionId, stepExecutionId, executeContext.CancellationToken);
                return result;
            }
            catch (Exception exception)
            {
                await processTrackingStore.FailStepAsync(processExecutionId, stepExecutionId, exception.Message, executeContext.CancellationToken);
                logger.LogError(exception, "Error uploading file {FilePath} to {Key}",
                    sourceFilePath, executeContext.Arguments.DestinationRoute);

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

        public async Task<CompensationResult> Compensate(CompensateContext<UploadFileToBucketLog> compensateContext)
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

                await s3Service.RemoveFileAsync(
                    compensateContext.Log.DestinationKey,
                    compensateContext.CancellationToken);
                return compensateContext.Compensated();
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error compensating upload of file to {Key}",
                    compensateContext.Log.DestinationKey);
                return compensateContext.Failed(exception);
            }
        }
    }
}

