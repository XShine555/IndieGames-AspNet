using Application.Abstractions.Persistence;
using Domain.JobTracking;
using Infrastructure.Messaging.Features.Common.Workflows;
using Infrastructure.Messaging.Features.Games.Workflows.StorePictureProcessing.Arguments;
using Infrastructure.Messaging.Features.Games.Workflows.StorePictureProcessing.Variables;
using Infrastructure.Messaging.Features.Users.Workflows.ProfilePictureProcessing.Variables;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging.Features.Users.Activities
{
    public class GenerateUserProfilePictureWorkflowPathsActivity(
        IJobTrackingStore processTrackingStore,
        ILogger<GenerateUserProfilePictureWorkflowPathsActivity> logger)
        : IExecuteActivity<GeneratePictureWorkflowPathsArguments>
    {
        public const string ExecuteEndpointName = "generate-user-profile-picture-workflow-paths";

        internal const string ResizedPictureFileExtension = ".webp";

        public async Task<ExecutionResult> Execute(ExecuteContext<GeneratePictureWorkflowPathsArguments> executeContext)
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
                var destinationFolderName = Guid.NewGuid().ToString();
                var workingDirectory = Path.Combine(executeContext.Arguments.TemporaryDirectory, destinationFolderName);

                var sourceFilePath = Path.Combine(
                    workingDirectory,
                    Guid.NewGuid() + Path.GetExtension(executeContext.Arguments.SourceKey));
                var smallPictureFilePath = Path.Combine(
                    workingDirectory,
                    Guid.NewGuid() + ResizedPictureFileExtension);
                var mediumPictureFilePath = Path.Combine(
                    workingDirectory,
                    Guid.NewGuid() + ResizedPictureFileExtension);
                var largePictureFilePath = Path.Combine(
                    workingDirectory,
                    Guid.NewGuid() + ResizedPictureFileExtension);

                Directory.CreateDirectory(workingDirectory);
                logger.LogDebug("Generated user profile picture workflow paths in {WorkingDirectory}", workingDirectory);
                logger.LogInformation(
                    "Generate user profile picture workflow paths activity completed for source {SourceKey} in {WorkingDirectory}",
                    executeContext.Arguments.SourceKey,
                    workingDirectory);

                var result = executeContext.CompletedWithVariables(new Dictionary<string, object>
                {
                    [GameStorePictureRoutingSlipVariableNames.Workflow.TemporalDirectory] = workingDirectory,
                    [UserProfilePictureRoutingSlipVariableNames.Picture.OriginalFilePath] = sourceFilePath,
                    [UserProfilePictureRoutingSlipVariableNames.Picture.SmallResizedFilePath] = smallPictureFilePath,
                    [UserProfilePictureRoutingSlipVariableNames.Picture.MediumResizedFilePath] = mediumPictureFilePath,
                    [UserProfilePictureRoutingSlipVariableNames.Picture.LargeResizedFilePath] = largePictureFilePath,
                } );

                await processTrackingStore.CompleteStepAsync(processExecutionId, stepExecutionId, executeContext.CancellationToken);
                return result;
            }
            catch (Exception exception)
            {
                await processTrackingStore.FailStepAsync(processExecutionId, stepExecutionId, exception.Message, executeContext.CancellationToken);
                logger.LogError(exception, "Failed to generate user profile picture workflow paths for {SourceKey}", executeContext.Arguments.SourceKey);
                throw;
            }
        }
    }
}
