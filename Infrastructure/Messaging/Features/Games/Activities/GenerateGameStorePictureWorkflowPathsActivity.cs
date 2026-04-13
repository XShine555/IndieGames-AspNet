using Infrastructure.Messaging.Features.Games.Workflows.StorePictureProcessing.Arguments;
using Infrastructure.Messaging.Features.Games.Workflows.StorePictureProcessing.Variables;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging.Features.Games.Activities
{
    public class GenerateGameStorePictureWorkflowPathsActivity(
        ILogger<GenerateGameStorePictureWorkflowPathsActivity> logger)
        : IExecuteActivity<GenerateGameStorePictureWorkflowPathsArguments>
    {
        public const string ExecuteEndpointName = "generate-game-store-picture-workflow-paths";

        internal const string ResizedPictureFileExtension = ".webp";

        public Task<ExecutionResult> Execute(ExecuteContext<GenerateGameStorePictureWorkflowPathsArguments> executeContext)
        {
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
                logger.LogDebug("Generated game store picture workflow paths in {WorkingDirectory}", workingDirectory);
                logger.LogInformation(
                    "Generate game store picture workflow paths activity completed for source {SourceKey} in {WorkingDirectory}",
                    executeContext.Arguments.SourceKey,
                    workingDirectory);

                return Task.FromResult(executeContext.CompletedWithVariables(new Dictionary<string, object>
                {
                    [GameStorePictureRoutingSlipVariableNames.Workflow.TemporalDirectory] = workingDirectory,
                    [GameStorePictureRoutingSlipVariableNames.Picture.OriginalFilePath] = sourceFilePath,
                    [GameStorePictureRoutingSlipVariableNames.Picture.SmallResizedFilePath] = smallPictureFilePath,
                    [GameStorePictureRoutingSlipVariableNames.Picture.MediumResizedFilePath] = mediumPictureFilePath,
                    [GameStorePictureRoutingSlipVariableNames.Picture.LargeResizedFilePath] = largePictureFilePath,
                    [GameStorePictureRoutingSlipVariableNames.Picture.DestinationFolderName] = destinationFolderName
                }));
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Failed to generate game store picture workflow paths for {SourceKey}", executeContext.Arguments.SourceKey);
                throw;
            }
        }
    }
}
