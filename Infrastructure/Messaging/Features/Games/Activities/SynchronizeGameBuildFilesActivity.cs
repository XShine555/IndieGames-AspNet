using Application.Abstractions.Persistence;
using Domain.Games.Entities;
using Domain.Games.Enums;
using Domain.JobTracking;
using Infrastructure.Messaging.Features.Common.Workflows;
using Infrastructure.Messaging.Features.Games.Workflows.BuildProcessing.Arguments;
using Infrastructure.Messaging.Features.Games.Workflows.BuildProcessing.Models;
using Infrastructure.Messaging.Features.Games.Workflows.BuildProcessing.Variables;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Infrastructure.Messaging.Features.Games.Activities
{
    public class SynchronizeGameBuildFilesActivity(
        IJobTrackingStore processTrackingStore,
        IDatabase database,
        ILogger<SynchronizeGameBuildFilesActivity> logger)
        : IExecuteActivity<SynchronizeGameBuildFilesArguments>
    {
        public const string ExecuteEndpointName = "synchronize-game-build-files";

        public async Task<ExecutionResult> Execute(ExecuteContext<SynchronizeGameBuildFilesArguments> executeContext)
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
                var manifestRelativePath = executeContext.GetVariable<string>(GameBuildRoutingSlipVariableNames.Build.ManifestRelativePath)
                    ?? throw new InvalidOperationException("Manifest relative path is required.");
                var manifestFileName = executeContext.GetVariable<string>(GameBuildRoutingSlipVariableNames.Build.ManifestFileName)
                    ?? throw new InvalidOperationException("Manifest file name is required.");
                var manifestContentType = executeContext.GetVariable<string>(GameBuildRoutingSlipVariableNames.Build.ManifestContentType)
                    ?? throw new InvalidOperationException("Manifest content type is required.");
                var filesJson = executeContext.GetVariable<string>(GameBuildRoutingSlipVariableNames.Build.FilesJson)
                    ?? throw new InvalidOperationException("Build files metadata is required.");

                var buildFiles = JsonSerializer.Deserialize<List<GameBuildFileMetadata>>(filesJson)
                    ?? throw new InvalidOperationException("Build files metadata is invalid.");

                var gameBuild = await database.GameBuilds
                    .SingleOrDefaultAsync(b => b.Id == executeContext.Arguments.BuildId && b.GameId == executeContext.Arguments.GameId,
                        executeContext.CancellationToken);

                if (gameBuild is null)
                {
                    throw new InvalidOperationException($"Build {executeContext.Arguments.BuildId} was not found.");
                }

                var existingBuildFiles = await database.GameBuildFiles
                    .Where(file => file.GameBuildId == gameBuild.Id)
                    .ToListAsync(executeContext.CancellationToken);

                if (existingBuildFiles.Count != 0)
                {
                    database.GameBuildFiles.RemoveRange(existingBuildFiles);
                }

                var gameBuildFiles = buildFiles.Select(file => new GameBuildFile
                {
                    Id = file.FileId,
                    GameBuildId = gameBuild.Id,
                    FileRelativePath = file.FileRelativePath,
                    FileName = file.FileName,
                    FileContentType = file.FileContentType,
                    FileSize = file.FileSize,
                    Hash = file.Hash,
                    HashAlgorithm = file.HashAlgorithm,
                } );

                await database.GameBuildFiles.AddRangeAsync(gameBuildFiles, executeContext.CancellationToken);

                gameBuild.ManifestRelativePath = manifestRelativePath;
                gameBuild.ManifestFileName = manifestFileName;
                gameBuild.ManifestContentType = manifestContentType;
                gameBuild.Status = GameBuildStatus.Completed;

                await database.SaveChangesAsync(executeContext.CancellationToken);

                await processTrackingStore.CompleteStepAsync(processExecutionId, stepExecutionId, executeContext.CancellationToken);
                await processTrackingStore.CompleteJobAsync(processExecutionId, executeContext.CancellationToken);

                logger.LogInformation(
                    "Build {BuildId} synchronized with {FilesCount} files and manifest {ManifestFileName}",
                    gameBuild.Id,
                    buildFiles.Count,
                    manifestFileName);

                return executeContext.Completed();
            }
            catch (Exception exception)
            {
                await processTrackingStore.FailStepAsync(processExecutionId, stepExecutionId, exception.Message, executeContext.CancellationToken);
                logger.LogError(exception, "Error synchronizing build files for build {BuildId}", executeContext.Arguments.BuildId);
                throw;
            }
        }
    }
}
