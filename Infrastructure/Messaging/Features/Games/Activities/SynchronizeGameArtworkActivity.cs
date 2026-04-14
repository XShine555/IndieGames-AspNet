using Application.Abstractions.Persistence;
using Domain.JobTracking;
using Infrastructure.Messaging.Features.Common.Workflows;
using Infrastructure.Messaging.Features.Games.Workflows.ArtworkProcessing.Arguments;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MimeMapping;

namespace Infrastructure.Messaging.Features.Games.Activities
{
    public class SynchronizeGameArtworkActivity(
        IProcessTrackingStore processTrackingStore,
        IDatabase database,
        ILogger<SynchronizeGameArtworkActivity> logger)
        : IExecuteActivity<SynchronizeGameArtworkArguments>
    {
        public const string ExecuteEndpointName = "synchronize-game-artwork";

        public async Task<ExecutionResult> Execute(ExecuteContext<SynchronizeGameArtworkArguments> executeContext)
        {
            var processExecutionIdValue = executeContext.GetVariable<string>(ProcessTrackingRoutingSlipVariableNames.Workflow.ProcessExecutionId)
                ?? throw new InvalidOperationException("Process execution id is required.");
            var processExecutionId = Guid.Parse(processExecutionIdValue);

            var stepExecutionId = await processTrackingStore.StartStepAsync(
                processExecutionId,
                ExecuteEndpointName,
                JobTrackingType.Activity,
                executeContext.CancellationToken);

            var smallResizedVariable = executeContext.GetVariable<string>(executeContext.Arguments.SmallPictureVariable);
            ArgumentNullException.ThrowIfNull(smallResizedVariable, nameof(smallResizedVariable));
            var mediumResizedVariable = executeContext.GetVariable<string>(executeContext.Arguments.MediumPictureVariable);
            ArgumentNullException.ThrowIfNull(mediumResizedVariable, nameof(mediumResizedVariable));
            var largeResizedVariable = executeContext.GetVariable<string>(executeContext.Arguments.LargePictureVariable);
            ArgumentNullException.ThrowIfNull(largeResizedVariable, nameof(largeResizedVariable));

            try
            {
                var artwork = await database.GameArtworks
                    .SingleOrDefaultAsync(x => x.Id == executeContext.Arguments.ArtworkId, executeContext.CancellationToken);

                if (artwork is null)
                {
                    logger.LogError("Game artwork with id {ArtworkId} not found while synchronizing generated variants",
                        executeContext.Arguments.ArtworkId);
                    throw new InvalidOperationException($"Game artwork with id {executeContext.Arguments.ArtworkId} not found.");
                }

                artwork.SmallFileName = Path.GetFileName(smallResizedVariable);
                artwork.SmallContentType = MimeUtility.GetMimeMapping(smallResizedVariable);
                artwork.SmallRelativePath = executeContext.Arguments.SmallRelativePath;
                artwork.SmallWidth = executeContext.Arguments.SmallWidth;
                artwork.SmallHeight = executeContext.Arguments.SmallHeight;
                artwork.SmallFileSizeInBytes = new FileInfo(smallResizedVariable).Length;

                artwork.MediumFileName = Path.GetFileName(mediumResizedVariable);
                artwork.MediumContentType = MimeUtility.GetMimeMapping(mediumResizedVariable);
                artwork.MediumRelativePath = executeContext.Arguments.MediumRelativePath;
                artwork.MediumWidth = executeContext.Arguments.MediumWidth;
                artwork.MediumHeight = executeContext.Arguments.MediumHeight;
                artwork.MediumFileSizeInBytes = new FileInfo(mediumResizedVariable).Length;

                artwork.LargeFileName = Path.GetFileName(largeResizedVariable);
                artwork.LargeContentType = MimeUtility.GetMimeMapping(largeResizedVariable);
                artwork.LargeRelativePath = executeContext.Arguments.LargeRelativePath;
                artwork.LargeWidth = executeContext.Arguments.LargeWidth;
                artwork.LargeHeight = executeContext.Arguments.LargeHeight;
                artwork.LargeFileSizeInBytes = new FileInfo(largeResizedVariable).Length;

                artwork.ProcessingStatus = Domain.Entities.GameArtworkProcessingStatus.Completed;
                artwork.ProcessingError = string.Empty;
                artwork.UpdatedAt = DateTime.UtcNow;

                await database.SaveChangesAsync(executeContext.CancellationToken);

                var hasPendingArtwork = await database.GameArtworks
                    .AsNoTracking()
                    .AnyAsync(
                        x => x.GameId == artwork.GameId && x.ProcessingStatus != Domain.Entities.GameArtworkProcessingStatus.Completed,
                        executeContext.CancellationToken);

                if (!hasPendingArtwork)
                {
                    var game = await database.Games
                        .SingleOrDefaultAsync(x => x.Id == artwork.GameId, executeContext.CancellationToken);

                    if (game is not null && game.StoreReadinessStatus != Domain.Entities.GameStoreReadinessStatus.ReadyForStore)
                    {
                        game.StoreReadinessStatus = Domain.Entities.GameStoreReadinessStatus.ReadyForStore;
                        await database.SaveChangesAsync(executeContext.CancellationToken);
                    }
                }

                logger.LogDebug("Synchronized generated variants for artwork {ArtworkId}", artwork.Id);
                logger.LogInformation("Synchronize game artwork activity completed for artwork {ArtworkId}", artwork.Id);

                var result = executeContext.Completed();
                await processTrackingStore.CompleteStepAsync(processExecutionId, stepExecutionId, executeContext.CancellationToken);
                await processTrackingStore.CompleteProcessAsync(processExecutionId, executeContext.CancellationToken);
                return result;
            }
            catch (Exception exception)
            {
                await processTrackingStore.FailStepAsync(processExecutionId, stepExecutionId, exception.Message, executeContext.CancellationToken);
                logger.LogError(exception,
                    "An error occurred while synchronizing generated variants for artwork id {ArtworkId}",
                    executeContext.Arguments.ArtworkId);
                throw;
            }
        }
    }
}

