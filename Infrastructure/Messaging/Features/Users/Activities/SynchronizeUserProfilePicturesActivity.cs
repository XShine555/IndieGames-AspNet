using Application.Abstractions.Persistence;
using Domain.JobTracking;
using Infrastructure.Messaging.Features.Common.Workflows;
using Infrastructure.Messaging.Features.Users.Workflows.ProfilePictureProcessing.Arguments;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging.Features.Users.Activities
{
    public class SynchronizeUserProfilePicturesActivity(
        IProcessTrackingStore processTrackingStore,
        IDatabase database,
        ILogger<SynchronizeUserProfilePicturesActivity> logger)
        : IExecuteActivity<SynchronizeUserProfilePicturesArguments>
    {
        public const string ExecuteEndpointName = "synchronize-user-profile-pictures";

        public async Task<ExecutionResult> Execute(ExecuteContext<SynchronizeUserProfilePicturesArguments> executeContext)
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
                var picture = await database.UserProfilePictures
                    .SingleOrDefaultAsync(p => p.Id == executeContext.Arguments.PictureId, executeContext.CancellationToken);

                if (picture is null)
                {
                    logger.LogError("User profile picture with id {PictureId} not found while synchronizing generated pictures",
                        executeContext.Arguments.PictureId);
                    throw new InvalidOperationException($"User profile picture with id {executeContext.Arguments.PictureId} not found.");
                }

                picture.SmallName = Path.GetFileName(smallResizedVariable);
                picture.SmallFileExtension = Path.GetExtension(smallResizedVariable);
                picture.SmallRelativePath = executeContext.Arguments.SmallRelativePath;

                picture.MediumName = Path.GetFileName(mediumResizedVariable);
                picture.MediumFileExtension = Path.GetExtension(mediumResizedVariable);
                picture.MediumRelativePath = executeContext.Arguments.MediumRelativePath;

                picture.LargeName = Path.GetFileName(largeResizedVariable);
                picture.LargeFileExtension = Path.GetExtension(largeResizedVariable);
                picture.LargeRelativePath = executeContext.Arguments.LargeRelativePath;

                await database.SaveChangesAsync(executeContext.CancellationToken);

                logger.LogDebug("Synchronized generated user profile pictures for picture {PictureId}", picture.Id);
                logger.LogInformation("Synchronize user profile pictures activity completed for picture {PictureId}", picture.Id);

                var result = executeContext.Completed();
                await processTrackingStore.CompleteStepAsync(processExecutionId, stepExecutionId, executeContext.CancellationToken);
                await processTrackingStore.CompleteProcessAsync(processExecutionId, executeContext.CancellationToken);
                return result;
            }
            catch (Exception exception)
            {
                await processTrackingStore.FailStepAsync(processExecutionId, stepExecutionId, exception.Message, executeContext.CancellationToken);
                logger.LogError(exception, "An error occurred while synchronizing generated user profile pictures for picture id {PictureId}",
                    executeContext.Arguments.PictureId);
                throw;
            }
        }
    }
}

