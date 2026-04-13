using Infrastructure.Messaging.Features.Games.Workflows.ArtworkProcessing.Arguments;
using Infrastructure.Messaging.Features.Games.Workflows.ArtworkProcessing.Variables;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging.Features.Games.Activities
{
    public class GenerateGameArtworkWorkflowPathsActivity(
        ILogger<GenerateGameArtworkWorkflowPathsActivity> logger)
        : IExecuteActivity<GenerateGameArtworkWorkflowPathsArguments>
    {
        public const string ExecuteEndpointName = "generate-game-artwork-workflow-paths";

        internal const string ResizedPictureFileExtension = ".webp";

        public Task<ExecutionResult> Execute(ExecuteContext<GenerateGameArtworkWorkflowPathsArguments> executeContext)
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
                logger.LogDebug("Generated game artwork workflow paths in {WorkingDirectory}", workingDirectory);
                logger.LogInformation(
                    "Generate game artwork workflow paths activity completed for source {SourceKey} in {WorkingDirectory}",
                    executeContext.Arguments.SourceKey,
                    workingDirectory);

                return Task.FromResult(executeContext.CompletedWithVariables(new Dictionary<string, object>
                {
                    [GameArtworkRoutingSlipVariableNames.Picture.OriginalFilePath] = sourceFilePath,
                    [GameArtworkRoutingSlipVariableNames.Picture.SmallResizedFilePath] = smallPictureFilePath,
                    [GameArtworkRoutingSlipVariableNames.Picture.MediumResizedFilePath] = mediumPictureFilePath,
                    [GameArtworkRoutingSlipVariableNames.Picture.LargeResizedFilePath] = largePictureFilePath
                }));
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Failed to generate game artwork workflow paths for {SourceKey}", executeContext.Arguments.SourceKey);
                throw;
            }
        }
    }
}
