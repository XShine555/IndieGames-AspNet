using Application.Abstractions.Persistence;
using Domain.Entities;
using Domain.JobTracking;
using Infrastructure.Messaging.Features.Common.Workflows;
using Infrastructure.Messaging.Features.Games.Workflows.StorePictureProcessing.Arguments;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MimeMapping;

namespace Infrastructure.Messaging.Features.Games.Activities
{
    public class SynchronizeGameStorePicturesActivity(
        IJobTrackingStore processTrackingStore,
        IDatabase database,
        ILogger<SynchronizeGameStorePicturesActivity> logger)
        : IExecuteActivity<SynchronizeGameStorePicturesArguments>
    {
        public const string ExecuteEndpointName = "synchronize-game-store-pictures";

        public async Task<ExecutionResult> Execute(ExecuteContext<SynchronizeGameStorePicturesArguments> executeContext)
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
                var picture = await database.GamePictures
                    .SingleOrDefaultAsync(p => p.Id == executeContext.Arguments.PictureId, executeContext.CancellationToken);

                if (picture is null)
                {
                    logger.LogError("Picture with id {PictureId} not found while synchronizing generated pictures",
                        executeContext.Arguments.PictureId);
                    throw new InvalidOperationException($"Picture with id {executeContext.Arguments.PictureId} not found.");
                }

                picture.SmallName = Path.GetFileName(smallResizedVariable);
                picture.SmallFileContentType = MimeUtility.GetMimeMapping(smallResizedVariable);
                picture.SmallRelativePath = picture.OriginalRelativePath;

                picture.MediumName = Path.GetFileName(mediumResizedVariable);
                picture.MediumContentType = MimeUtility.GetMimeMapping(mediumResizedVariable);
                picture.MediumRelativePath = picture.OriginalRelativePath;

                picture.LargeName = Path.GetFileName(largeResizedVariable);
                picture.LargeContentType = MimeUtility.GetMimeMapping(largeResizedVariable);
                picture.LargeRelativePath = picture.OriginalRelativePath;

                picture.ProcessingStatus = GamePictureProcessingStatus.Completed;

                await database.SaveChangesAsync(executeContext.CancellationToken);

                logger.LogDebug("Synchronized generated pictures for picture {PictureId}",
                    picture.Id);
                logger.LogInformation("Synchronize game store pictures activity completed for picture {PictureId}",
                    picture.Id);

                var result = executeContext.Completed();
                await processTrackingStore.CompleteStepAsync(processExecutionId, stepExecutionId, executeContext.CancellationToken);
                await processTrackingStore.CompleteJobAsync(processExecutionId, executeContext.CancellationToken);
                return result;
            }
            catch (Exception exception)
            {
                await processTrackingStore.FailStepAsync(processExecutionId, stepExecutionId, exception.Message, executeContext.CancellationToken);
                logger.LogError(exception, "An error occurred while synchronizing generated pictures for picture id {PictureId}",
                    executeContext.Arguments.PictureId);
                throw;
            }
        }
    }
}

