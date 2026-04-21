using Amazon.S3;
using Amazon.S3.Model;
using Application.Abstractions.Persistence;
using Domain.Games.Enums;
using Domain.JobTracking;
using Infrastructure.Configurations;
using Infrastructure.Messaging.Features.Common.Workflows;
using Infrastructure.Messaging.Features.Games.Workflows.BuildRemoval.Arguments;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging.Features.Games.Activities
{
    public class RemoveGameBuildFilesFromStorageActivity(
        IJobTrackingStore processTrackingStore,
        IDatabase database,
        IAmazonS3 amazonS3,
        S3Configuration s3Configuration,
        ILogger<RemoveGameBuildFilesFromStorageActivity> logger)
        : IExecuteActivity<RemoveGameBuildFilesFromStorageArguments>
    {
        public const string ExecuteEndpointName = "remove-game-build-files-from-storage";

        public async Task<ExecutionResult> Execute(ExecuteContext<RemoveGameBuildFilesFromStorageArguments> executeContext)
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
                var storagePrefix = executeContext.Arguments.BuildStoragePath.TrimEnd('/');
                if (string.IsNullOrWhiteSpace(storagePrefix))
                {
                    throw new InvalidOperationException("Build storage path is required.");
                }

                await DeleteByPrefixAsync(storagePrefix, executeContext.CancellationToken);

                logger.LogInformation(
                    "Removed storage files for build {BuildId} under prefix {StoragePrefix}",
                    executeContext.Arguments.BuildId,
                    storagePrefix);

                await processTrackingStore.CompleteStepAsync(processExecutionId, stepExecutionId, executeContext.CancellationToken);
                return executeContext.Completed();
            }
            catch (Exception exception)
            {
                await MarkBuildAsFailedAsync(executeContext.Arguments.BuildId, executeContext.CancellationToken);
                await processTrackingStore.FailStepAsync(processExecutionId, stepExecutionId, exception.Message, executeContext.CancellationToken);

                logger.LogError(
                    exception,
                    "Error removing storage files for build {BuildId}",
                    executeContext.Arguments.BuildId);

                throw;
            }
        }

        private async Task DeleteByPrefixAsync(string storagePrefix, CancellationToken cancellationToken)
        {
            string? continuationToken = null;

            do
            {
                var listResponse = await amazonS3.ListObjectsV2Async(new ListObjectsV2Request
                {
                    BucketName = s3Configuration.BucketName,
                    Prefix = storagePrefix,
                    ContinuationToken = continuationToken,
                }, cancellationToken);

                if (listResponse.S3Objects.Count != 0)
                {
                    var deleteRequest = new DeleteObjectsRequest
                    {
                        BucketName = s3Configuration.BucketName,
                        Objects = listResponse.S3Objects
                            .Select(s3Object => new KeyVersion { Key = s3Object.Key })
                            .ToList(),
                    };

                    await amazonS3.DeleteObjectsAsync(deleteRequest, cancellationToken);
                }

                continuationToken = listResponse.IsTruncated == true
                    ? listResponse.NextContinuationToken
                    : null;
            }
            while (!string.IsNullOrWhiteSpace(continuationToken));
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
