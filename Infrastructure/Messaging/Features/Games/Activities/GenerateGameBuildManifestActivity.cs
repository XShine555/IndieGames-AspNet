using Amazon.S3;
using Amazon.S3.Model;
using Application.Abstractions.Persistence;
using Application.Abstractions.Storage;
using Domain.JobTracking;
using Infrastructure.Configurations;
using Infrastructure.Messaging.Features.Common.Workflows;
using Infrastructure.Messaging.Features.Games.Workflows.BuildProcessing.Arguments;
using Infrastructure.Messaging.Features.Games.Workflows.BuildProcessing.Models;
using Infrastructure.Messaging.Features.Games.Workflows.BuildProcessing.Variables;
using MassTransit;
using Microsoft.Extensions.Logging;
using MimeMapping;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Messaging.Features.Games.Activities
{
    public class GenerateGameBuildManifestActivity(
        IJobTrackingStore processTrackingStore,
        IAmazonS3 amazonS3,
        IS3Service s3Service,
        S3Configuration s3Configuration,
        ILogger<GenerateGameBuildManifestActivity> logger)
        : IExecuteActivity<GenerateGameBuildManifestArguments>
    {
        public const string ExecuteEndpointName = "generate-game-build-manifest";

        private const string ManifestContentType = "application/json";
        private const string HashAlgorithmName = "SHA256";

        public async Task<ExecutionResult> Execute(ExecuteContext<GenerateGameBuildManifestArguments> executeContext)
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
                var manifestFileName = $"{executeContext.Arguments.BuildId:D}-Manifest.json";
                var buildStoragePath = executeContext.Arguments.BuildStoragePath.TrimEnd('/');

                var discoveredFiles = await DiscoverFilesAsync(
                    buildStoragePath,
                    manifestFileName,
                    executeContext.CancellationToken);

                var manifest = new GameBuildManifest(
                    discoveredFiles.Select(file => new GameBuildManifestFile(
                        file.FileName,
                        file.FileContentType,
                        file.FileSize,
                        file.Hash,
                        file.HashAlgorithm)).ToList());

                var manifestJson = JsonSerializer.Serialize(manifest, new JsonSerializerOptions
                {
                    WriteIndented = true,
                } );

                var manifestKey = $"{buildStoragePath}/{manifestFileName}";
                await using var manifestStream = new MemoryStream(Encoding.UTF8.GetBytes(manifestJson));
                await s3Service.UploadFileAsync(
                    manifestStream,
                    manifestKey,
                    ManifestContentType,
                    executeContext.CancellationToken);

                logger.LogInformation(
                    "Generated manifest {ManifestFileName} for build {BuildId} with {FilesCount} files",
                    manifestFileName,
                    executeContext.Arguments.BuildId,
                    discoveredFiles.Count);

                var result = executeContext.CompletedWithVariables(new Dictionary<string, object>
                {
                    [GameBuildRoutingSlipVariableNames.Build.ManifestRelativePath] = buildStoragePath,
                    [GameBuildRoutingSlipVariableNames.Build.ManifestFileName] = manifestFileName,
                    [GameBuildRoutingSlipVariableNames.Build.ManifestContentType] = ManifestContentType,
                    [GameBuildRoutingSlipVariableNames.Build.FilesJson] = JsonSerializer.Serialize(discoveredFiles)
                } );

                await processTrackingStore.CompleteStepAsync(processExecutionId, stepExecutionId, executeContext.CancellationToken);
                return result;
            }
            catch (Exception exception)
            {
                await processTrackingStore.FailStepAsync(processExecutionId, stepExecutionId, exception.Message, executeContext.CancellationToken);
                logger.LogError(exception,
                    "Error generating game build manifest for build {BuildId}",
                    executeContext.Arguments.BuildId);
                throw;
            }
        }

        private async Task<List<GameBuildFileMetadata>> DiscoverFilesAsync(
            string buildStoragePath,
            string manifestFileName,
            CancellationToken cancellationToken)
        {
            var files = new List<GameBuildFileMetadata>();
            string? continuationToken = null;

            do
            {
                var listResponse = await amazonS3.ListObjectsV2Async(new ListObjectsV2Request
                {
                    BucketName = s3Configuration.BucketName,
                    Prefix = buildStoragePath,
                    ContinuationToken = continuationToken
                }, cancellationToken);

                foreach (var s3Object in listResponse.S3Objects)
                {
                    if (s3Object.Key.EndsWith("/", StringComparison.Ordinal))
                        continue;

                    var fileName = Path.GetFileName(s3Object.Key);
                    if (string.Equals(fileName, manifestFileName, StringComparison.OrdinalIgnoreCase))
                        continue;

                    var contentType = await GetContentTypeAsync(s3Object.Key, cancellationToken);
                    await using var fileStream = await s3Service.GetFileStreamAsync(s3Object.Key, cancellationToken);
                    var hashBytes = await SHA256.HashDataAsync(fileStream, cancellationToken);
                    var hash = Convert.ToHexString(hashBytes).ToLowerInvariant();

                    var fileRelativePath = Path.GetDirectoryName(s3Object.Key)?.Replace('\\', '/') ?? string.Empty;

                    files.Add(new GameBuildFileMetadata(
                        fileRelativePath,
                        fileName,
                        contentType,
                        s3Object.Size ?? 0,
                        hash,
                        HashAlgorithmName));
                }

                continuationToken = listResponse.IsTruncated == true
                    ? listResponse.NextContinuationToken
                    : null;
            }
            while (!string.IsNullOrWhiteSpace(continuationToken));

            return files;
        }

        private async Task<string> GetContentTypeAsync(string key, CancellationToken cancellationToken)
        {
            var metadata = await amazonS3.GetObjectMetadataAsync(new GetObjectMetadataRequest
            {
                BucketName = s3Configuration.BucketName,
                Key = key,
            }, cancellationToken);

            return string.IsNullOrWhiteSpace(metadata.Headers.ContentType)
                ? MimeUtility.GetMimeMapping(key)
                : metadata.Headers.ContentType;
        }

        private sealed record GameBuildManifest(List<GameBuildManifestFile> Files);

        private sealed record GameBuildManifestFile(
            string Name,
            string ContentType,
            long Size,
            string Hash,
            string HashAlgorithm);
    }
}
