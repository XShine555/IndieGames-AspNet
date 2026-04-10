using Application.Contracts.Infrastructure;
using Automatonymous;
using Infrastructure.MassTransit.Arguments;
using Infrastructure.MassTransit.Logs;
using MassTransit;
using Microsoft.Extensions.Logging;
using MimeMapping;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Infrastructure.MassTransit.Activities.Files
{
    public class UploadFileToBucketActivity(IS3Service s3Service,
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
                    MimeUtility.GetMimeMapping(fileName),
                    $"{executeContext.Arguments.DestinationRoute}/{fileName}",
                    executeContext.CancellationToken);

                return executeContext.CompletedWithVariables(new UploadFileToBucketLog(
                    executeContext.Arguments.DestinationRoute));
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error uploading file {FilePath} to bucket at {Key}",
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
                logger.LogError(exception, "Error compensating upload of file to bucket at {Key}",
                    compensateContext.Log.DestinationKey);
                return compensateContext.Failed(exception);
            }
        }
    }
}
