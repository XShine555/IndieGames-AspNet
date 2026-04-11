using Infrastructure.Messaging.Features.Games.Workflows.PictureProcessing.Arguments;
using Infrastructure.Messaging.Features.Games.Workflows.PictureProcessing.Variables;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging.Features.Games.Activities
{
    public class GeneratePictureWorkflowPathsActivity(
        ILogger<GeneratePictureWorkflowPathsActivity> logger)
        : IExecuteActivity<GeneratePictureWorkflowPathsArguments>
    {
        public const string ExecuteEndpointName = "generate-picture-workflow-paths";

        internal const String ResizedPictureFileExtension = ".webp";

        public async Task<ExecutionResult> Execute(ExecuteContext<GeneratePictureWorkflowPathsArguments> executeContext)
        {
            try
            {
                var destinationFolderName = Guid.NewGuid().ToString();
                var workingDirectory = Path.Combine(executeContext.Arguments.TemporaryDirectory, destinationFolderName);

                var sourceFilePath = Path.Combine(
                    workingDirectory, Guid.NewGuid().ToString() + Path.GetExtension(executeContext.Arguments.SourceKey));
                var smallPictureFilePath = Path.Combine(
                    workingDirectory, Guid.NewGuid().ToString() + ResizedPictureFileExtension);
                var mediumPictureFilePath = Path.Combine(
                    workingDirectory, Guid.NewGuid().ToString() + ResizedPictureFileExtension);
                var largePictureFilePath = Path.Combine(
                    workingDirectory, Guid.NewGuid().ToString() + ResizedPictureFileExtension);

                Directory.CreateDirectory(workingDirectory);
                logger.LogDebug("Generated picture workflow paths in {WorkingDirectory}",
                    workingDirectory);
                logger.LogInformation("Generate workflow paths activity completed for source {SourceKey} in {WorkingDirectory}",
                    executeContext.Arguments.SourceKey,
                    workingDirectory);

                return executeContext.CompletedWithVariables(new Dictionary<string, object>
                {
                    [RoutingSlipVariableNames.Workflow.TemporalDirectory] = workingDirectory,
                    [RoutingSlipVariableNames.Picture.OriginalFilePath] = sourceFilePath,
                    [RoutingSlipVariableNames.Picture.SmallResizedFilePath] = smallPictureFilePath,
                    [RoutingSlipVariableNames.Picture.MediumResizedFilePath] = mediumPictureFilePath,
                    [RoutingSlipVariableNames.Picture.LargeResizedFilePath] = largePictureFilePath,
                    [RoutingSlipVariableNames.Picture.DestinationFolderName] = destinationFolderName
                });
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Failed to generate picture workflow paths for {SourceKey}",
                    executeContext.Arguments.SourceKey);
                throw;
            }
        }
    }
}
