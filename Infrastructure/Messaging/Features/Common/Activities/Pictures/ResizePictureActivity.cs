using Application.Abstractions;
using Infrastructure.Messaging.Features.Games.Workflows.StorePictureProcessing.Arguments;
using Infrastructure.Messaging.Features.Games.Workflows.StorePictureProcessing.Logs;
using MassTransit;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;

namespace Infrastructure.Messaging.Features.Common.Activities.Pictures
{
    public class ResizePictureActivity(
        IPictureService pictureService,
        ILogger<ResizePictureActivity> logger)
        : IActivity<ResizePictureLocalArguments, ResizePictureLog>
    {
        public const string ExecuteEndpointName = "resize-picture";

        public Task<CompensationResult> Compensate(CompensateContext<ResizePictureLog> compensateContext)
        {
            try
            {
                File.Delete(compensateContext.Log.DestinationFilePath);
                return Task.FromResult(compensateContext.Compensated());
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Failed to compensate resized file {DestinationFilePath}", compensateContext.Log.DestinationFilePath);
                return Task.FromResult(compensateContext.Failed(exception));
            }
        }

        public async Task<ExecutionResult> Execute(ExecuteContext<ResizePictureLocalArguments> executeContext)
        {
            var sourceFilePath = executeContext.GetVariable<string>(executeContext.Arguments.SourceFilePathVariable);
            ArgumentNullException.ThrowIfNull(sourceFilePath, nameof(sourceFilePath));
            var destinationFilePath = executeContext.GetVariable<string>(executeContext.Arguments.DestinationFilePathVariable);
            ArgumentNullException.ThrowIfNull(destinationFilePath, nameof(destinationFilePath));

            try
            {
                if (!File.Exists(sourceFilePath))
                {
                    logger.LogWarning("Source file not found: {SourceFilePath}", sourceFilePath);
                    throw new FileNotFoundException($"Source file not found: {sourceFilePath}");
                }
                await using var fileStream = File.OpenRead(sourceFilePath);
                var resizedPicture = await pictureService.ResizePictureAsWebpAsync(
                    fileStream,
                    new Size(executeContext.Arguments.Width, executeContext.Arguments.Height),
                    executeContext.CancellationToken);

                var destinationDirectory = Path.GetDirectoryName(destinationFilePath);
                if (!string.IsNullOrEmpty(destinationDirectory))
                    Directory.CreateDirectory(destinationDirectory);

                await using var destinationStream = File.Create(destinationFilePath);
                await resizedPicture.CopyToAsync(destinationStream, executeContext.CancellationToken);
                logger.LogDebug("Resized picture from {SourceFilePath} to {DestinationFilePath}",
                    sourceFilePath,
                    destinationFilePath);
                logger.LogInformation("Resize activity completed for {SourceFilePath} into {DestinationFilePath}",
                    sourceFilePath,
                    destinationFilePath);

                return executeContext.Completed(new ResizePictureLog(destinationFilePath));
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "An error occurred while resizing the picture from {SourceFilePath} to {DestinationFilePath} with width {Width} and height {Height}.",
                    sourceFilePath, destinationFilePath, executeContext.Arguments.Width, executeContext.Arguments.Height);
                throw;
            }
        }
    }
}
