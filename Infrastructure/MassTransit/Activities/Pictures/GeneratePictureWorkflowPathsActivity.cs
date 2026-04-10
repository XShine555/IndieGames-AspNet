using Infrastructure.MassTransit.Arguments;
using Infrastructure.MassTransit.RoutingSlip.Variables;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.MassTransit.Activities.Pictures
{
    public class GeneratePictureWorkflowPathsActivity(
        ILogger<GeneratePictureWorkflowPathsActivity> logger)
        : IExecuteActivity<GeneratePictureWorkflowPathsArguments>
    {
        public const string ExecuteEndpointName = "generate-picture-workflow-paths";

        internal const string ResizedPictureFileExtension = ".webp";

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

                return executeContext.CompletedWithVariables(new Dictionary<string, object>
                {
                    [RoutingSlipVariableNames.Workflow.TemporalDirectory] = workingDirectory,
                    [RoutingSlipVariableNames.Picture.OriginalFilePath] = sourceFilePath,
                    [RoutingSlipVariableNames.Picture.SmallResizedFilePath] = smallPictureFilePath,
                    [RoutingSlipVariableNames.Picture.MediumResizedFilePath] = mediumPictureFilePath,
                    [RoutingSlipVariableNames.Picture.LargeResizedFilePath] = largePictureFilePath,
                    [RoutingSlipVariableNames.Picture.DestinationFolderName] = destinationFolderName
                } );
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