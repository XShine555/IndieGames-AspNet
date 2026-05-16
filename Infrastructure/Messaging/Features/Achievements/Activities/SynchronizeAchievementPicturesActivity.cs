using Application.Abstractions.Persistence;
using Domain.Games.Enums;
using Domain.JobTracking;
using Infrastructure.Messaging.Features.Common.Workflows;
using Infrastructure.Messaging.Features.Achievements.Workflows.AchievementPictureProcessing.Arguments;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MimeMapping;

namespace Infrastructure.Messaging.Features.Achievements.Activities
{
    public class SynchronizeAchievementPicturesActivity(
        IJobTrackingStore processTrackingStore,
        IDatabase database,
        ILogger<SynchronizeAchievementPicturesActivity> logger)
        : IExecuteActivity<SynchronizeAchievementPicturesArguments>
    {
        public const string ExecuteEndpointName = "synchronize-achievement-pictures";

        public async Task<ExecutionResult> Execute(ExecuteContext<SynchronizeAchievementPicturesArguments> executeContext)
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
                var picture = await database.AchievementPictures
                    .SingleOrDefaultAsync(p => p.AchievementId == executeContext.Arguments.PictureId, executeContext.CancellationToken);

                if (picture is null)
                {
                    logger.LogError("Achievement picture with id {PictureId} not found while synchronizing generated pictures",
                        executeContext.Arguments.PictureId);
                    throw new InvalidOperationException($"Achievement picture with id {executeContext.Arguments.PictureId} not found.");
                }

                picture.SmallName = Path.GetFileName(smallResizedVariable);
                picture.SmallFileContentType = MimeUtility.GetMimeMapping(smallResizedVariable);
                picture.SmallRelativePath = executeContext.Arguments.SmallRelativePath;

                picture.MediumName = Path.GetFileName(mediumResizedVariable);
                picture.MediumFileContentType = MimeUtility.GetMimeMapping(mediumResizedVariable);
                picture.MediumRelativePath = executeContext.Arguments.MediumRelativePath;

                picture.LargeName = Path.GetFileName(largeResizedVariable);
                picture.LargeContentType = MimeUtility.GetMimeMapping(largeResizedVariable);
                picture.LargeRelativePath = executeContext.Arguments.LargeRelativePath;

                picture.ProcessingStatus = AchievementPictureProcessingStatus.Completed;

                await database.SaveChangesAsync(executeContext.CancellationToken);

                logger.LogDebug("Synchronized generated achievement pictures for picture {PictureId}", picture.Id);
                logger.LogInformation("Synchronize achievement pictures activity completed for picture {PictureId}", picture.Id);

                var result = executeContext.Completed();
                await processTrackingStore.CompleteStepAsync(processExecutionId, stepExecutionId, executeContext.CancellationToken);
                await processTrackingStore.CompleteJobAsync(processExecutionId, executeContext.CancellationToken);
                return result;
            }
            catch (Exception exception)
            {
                await processTrackingStore.FailStepAsync(processExecutionId, stepExecutionId, exception.Message, executeContext.CancellationToken);
                logger.LogError(exception, "An error occurred while synchronizing generated achievement pictures for picture id {PictureId}",
                    executeContext.Arguments.PictureId);
                throw;
            }
        }
    }
}
