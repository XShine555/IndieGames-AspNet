using Application.Abstractions;
using Infrastructure.Messaging.Features.Games.Workflows.PictureProcessing.Arguments;
using Infrastructure.Messaging.Features.Games.Workflows.PictureProcessing.Logs;
using MassTransit;
using Microsoft.Extensions.Logging;
using MimeMapping;

namespace Infrastructure.Messaging.Features.Common.Activities.Files
{
    public class UploadFileToBucketActivity(
        IS3Service s3Service,
        ILogger<UploadFileToBucketActivity> logger)
        : IActivity<UploadFileToBucketArguments, UploadFileToBucketLog>
    {
        public const string ExecuteEndpointName = "upload-file-to-bucket";

        public async Task<ExecutionResult> Execute(ExecuteContext<UploadFileToBucketArguments> executeContext)
        {
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

                return executeContext.CompletedWithVariables(new UploadFileToBucketLog(
                    executeContext.Arguments.DestinationRoute));
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error uploading file {FilePath} to {Key}",
                    sourceFilePath, executeContext.Arguments.DestinationRoute);
                throw;
            }
        }

        public async Task<CompensationResult> Compensate(CompensateContext<UploadFileToBucketLog> compensateContext)
        {
            try
            {
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
