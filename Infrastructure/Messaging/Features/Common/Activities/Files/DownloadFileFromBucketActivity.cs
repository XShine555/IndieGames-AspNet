using Application.Abstractions.Persistence;
using Application.Abstractions.Storage;
using Domain.JobTracking;
using Infrastructure.Messaging.Features.Common.Workflows;
using Infrastructure.Messaging.Features.Games.Workflows.StorePictureProcessing.Arguments;
using Infrastructure.Messaging.Features.Games.Workflows.StorePictureProcessing.Logs;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging.Features.Common.Activities.Files
{
    public class DownloadFileFromBucketActivity(
        IJobTrackingStore processTrackingStore,
        IS3Service s3Service,
        IDatabase database,
        ILogger<DownloadFileFromBucketActivity> logger)
         : IActivity<DownloadFileFromBucketArguments, DownloadFileFromBucketLog>
    {
        public const string ExecuteEndpointName = "download-file-from-bucket";

        public async Task<CompensationResult> Compensate(CompensateContext<DownloadFileFromBucketLog> compensateContext)
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
                logger.LogError(exception, "Failed to compensate DownloadFileFromBucketActivity");
                return compensateContext.Failed();
            }
        }

        public async Task<ExecutionResult> Execute(ExecuteContext<DownloadFileFromBucketArguments> executeContext)
        {
            var processExecutionIdValue = executeContext.GetVariable<string>(ProcessTrackingRoutingSlipVariableNames.Workflow.ProcessExecutionId)
                ?? throw new InvalidOperationException("Process execution id is required.");
            var processExecutionId = Guid.Parse(processExecutionIdValue);

            var stepExecutionId = await processTrackingStore.StartStepAsync(
                processExecutionId,
                ExecuteEndpointName,
                JobTrackingType.Activity,
                executeContext.CancellationToken);

            var destinationPath = executeContext.GetVariable<string>(executeContext.Arguments.DestinationFilePathVariable);
            ArgumentNullException.ThrowIfNull(destinationPath, nameof(destinationPath));

            try
            {
                using var fileStream = await s3Service.GetFileStreamAsync(
                    executeContext.Arguments.Key,
                    executeContext.CancellationToken);

                var destinationDirectory = Path.GetDirectoryName(destinationPath);
                if (!string.IsNullOrWhiteSpace(destinationDirectory))
                    Directory.CreateDirectory(destinationDirectory);

                if (fileStream.CanSeek)
                    fileStream.Position = 0;

                using var destinationStream = File.Create(destinationPath);
                await fileStream.CopyToAsync(destinationStream, executeContext.CancellationToken);

                logger.LogInformation("Download activity completed for key {Key} into {DestinationPath}",
                    executeContext.Arguments.Key,
                    destinationPath);

                var result = executeContext.Completed(new DownloadFileFromBucketLog(
                    executeContext.Arguments.Key,
                    destinationPath));

                await processTrackingStore.CompleteStepAsync(processExecutionId, stepExecutionId, executeContext.CancellationToken);
                return result;
            }
            catch (Exception exception)
            {
                await processTrackingStore.FailStepAsync(processExecutionId, stepExecutionId, exception.Message, executeContext.CancellationToken);
                logger.LogError(exception, "Failed to download file from bucket");

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

