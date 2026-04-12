using Application.Abstractions.Persistence;
using Domain.Entities;
using Infrastructure.Messaging.Features.Games.Workflows.StorePictureProcessing.Arguments;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging.Features.Games.Activities
{
    public class SynchronizeGameStorePicturesActivity(
        IDatabase database,
        ILogger<SynchronizeGameStorePicturesActivity> logger)
        : IExecuteActivity<SynchronizeGameStorePicturesArguments>
    {
        public const string ExecuteEndpointName = "synchronize-game-store-pictures";

        public async Task<ExecutionResult> Execute(ExecuteContext<SynchronizeGameStorePicturesArguments> executeContext)
        {
            var smallResizedVariable = executeContext.GetVariable<string>(executeContext.Arguments.SmallPictureVariable);
            ArgumentNullException.ThrowIfNull(smallResizedVariable, nameof(smallResizedVariable));
            var mediumResizedVariable = executeContext.GetVariable<string>(executeContext.Arguments.MediumPictureVariable);
            ArgumentNullException.ThrowIfNull(mediumResizedVariable, nameof(mediumResizedVariable));
            var largeResizedVariable = executeContext.GetVariable<string>(executeContext.Arguments.LargePictureVariable);
            ArgumentNullException.ThrowIfNull(largeResizedVariable, nameof(largeResizedVariable));

            try
            {
                var picture = await database.GamePictures
                    .SingleOrDefaultAsync(p => p.Id == executeContext.Arguments.PictureId, executeContext.CancellationToken);

                if (picture is null)
                {
                    logger.LogError("Picture with id {PictureId} not found while synchronizing generated pictures",
                        executeContext.Arguments.PictureId);
                    throw new InvalidOperationException($"Picture with id {executeContext.Arguments.PictureId} not found.");
                }

                picture.SmallName = Path.GetFileName(smallResizedVariable);
                picture.SmallFileExtension = Path.GetExtension(smallResizedVariable);
                picture.SmallRelativePath = picture.RelativePath;

                picture.MediumName = Path.GetFileName(mediumResizedVariable);
                picture.MediumFileExtension = Path.GetExtension(mediumResizedVariable);
                picture.MediumRelativePath = picture.RelativePath;

                picture.LargeName = Path.GetFileName(largeResizedVariable);
                picture.LargeFileExtension = Path.GetExtension(largeResizedVariable);
                picture.LargeRelativePath = picture.RelativePath;

                picture.ProcessingStatus = GamePictureProcessingStatus.Completed;

                await database.SaveChangesAsync(executeContext.CancellationToken);

                logger.LogDebug("Synchronized generated pictures for picture {PictureId}",
                    picture.Id);
                logger.LogInformation("Synchronize game store pictures activity completed for picture {PictureId}",
                    picture.Id);

                return executeContext.Completed();
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "An error occurred while synchronizing generated pictures for picture id {PictureId}",
                    executeContext.Arguments.PictureId);
                throw;
            }
        }
    }
}
