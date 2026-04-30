using Application.Abstractions.Persistence;
using Domain.JobTracking;
using Infrastructure.Messaging.Features.Common.Workflows;
using Infrastructure.Messaging.Features.Achievements.Workflows.AchievementPictureProcessing.Arguments;
using Infrastructure.Messaging.Features.Achievements.Workflows.AchievementPictureProcessing.Variables;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging.Features.Achievements.Activities
{
    public class GenerateAchievementPictureWorkflowPathsActivity(
        global::Application.Abstractions.Persistence.IJobTrackingStore processTrackingStore,
        ILogger<GenerateAchievementPictureWorkflowPathsActivity> logger)
        : IExecuteActivity<GenerateAchievementPictureWorkflowPathsArguments>
    {
        public const string ExecuteEndpointName = "generate-achievement-picture-workflow-paths";

        internal const string ResizedPictureFileExtension = ".webp";

        public async Task<ExecutionResult> Execute(ExecuteContext<GenerateAchievementPictureWorkflowPathsArguments> executeContext)
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
                logger.LogDebug("Generated achievement picture workflow paths in {WorkingDirectory}", workingDirectory);
                logger.LogInformation(
                    "Generate achievement picture workflow paths activity completed for source {SourceKey} in {WorkingDirectory}",
                    executeContext.Arguments.SourceKey,
                    workingDirectory);

                var result = executeContext.CompletedWithVariables(new Dictionary<string, object>
                {
                    [AchievementPictureRoutingSlipVariableNames.Workflow.TemporalDirectory] = workingDirectory,
                    [AchievementPictureRoutingSlipVariableNames.Picture.OriginalFilePath] = sourceFilePath,
                    [AchievementPictureRoutingSlipVariableNames.Picture.SmallResizedFilePath] = smallPictureFilePath,
                    [AchievementPictureRoutingSlipVariableNames.Picture.MediumResizedFilePath] = mediumPictureFilePath,
                    [AchievementPictureRoutingSlipVariableNames.Picture.LargeResizedFilePath] = largePictureFilePath,
                });

                await processTrackingStore.CompleteStepAsync(processExecutionId, stepExecutionId, executeContext.CancellationToken);
                return result;
            }
            catch (Exception exception)
            {
                await processTrackingStore.FailStepAsync(processExecutionId, stepExecutionId, exception.Message, executeContext.CancellationToken);
                logger.LogError(exception, "Failed to generate achievement picture workflow paths for {SourceKey}", executeContext.Arguments.SourceKey);
                throw;
            }
        }
    }
}
