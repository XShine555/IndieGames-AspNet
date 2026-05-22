using Application.Abstractions.Persistence;
using Application.Abstractions.Storage;
using Domain.JobTracking;
using Infrastructure.Messaging.Features.Common.Workflows;
using Infrastructure.Messaging.Features.Games.Workflows.StorePictureProcessing.Arguments;
using Infrastructure.Messaging.Features.Games.Workflows.StorePictureProcessing.Logs;
using MassTransit;
using Microsoft.Extensions.Logging;
using MimeMapping;

namespace Infrastructure.Messaging.Features.Common.Activities.Files
{
    public class UploadFileToBucketActivity(
        IJobTrackingStore processTrackingStore,
        IS3Service s3Service,
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
                if (executeContext.Arguments.OnError is not null)
                {
                    await executeContext.Arguments.OnError.Invoke(executeContext.CancellationToken);
                }
                throw;
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

