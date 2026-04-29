using Application.Abstractions.Storage;
using Application.Abstractions.Common;
using Infrastructure.Messaging.Helpers;
using MassTransit;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;

namespace Infrastructure.Messaging.Features.Achievements.Activities
{
    public class ProcessAchievementPictureActivity(
        IS3Service s3Service,
        IPictureService pictureService,
        ILogger<ProcessAchievementPictureActivity> logger)
        : IActivity<ProcessAchievementPictureArguments, ProcessAchievementPictureLog>
    {
        public const string ExecuteEndpointName = "process-achievement-picture";

        public async Task<CompensationResult> Compensate(CompensateContext<ProcessAchievementPictureLog> compensateContext)
        {
            try
            {
                await RemoveFileIfExistsAsync(compensateContext.Log.SmallDestinationKey, compensateContext.CancellationToken);
                await RemoveFileIfExistsAsync(compensateContext.Log.MediumDestinationKey, compensateContext.CancellationToken);
                await RemoveFileIfExistsAsync(compensateContext.Log.LargeDestinationKey, compensateContext.CancellationToken);

                return compensateContext.Compensated();
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Failed to compensate achievement picture processing for destinations {SmallDestinationKey}, {MediumDestinationKey}, {LargeDestinationKey}",
                    compensateContext.Log.SmallDestinationKey,
                    compensateContext.Log.MediumDestinationKey,
                    compensateContext.Log.LargeDestinationKey);
                return compensateContext.Failed(exception);
            }
        }

        public async Task<ExecutionResult> Execute(ExecuteContext<ProcessAchievementPictureArguments> executeContext)
        {
            var sourceKey = executeContext.Arguments.SourceKey;
            var uploadedKeys = new List<string>();

            try
            {
                await using var sourceStream = await s3Service.GetFileStreamAsync(sourceKey, executeContext.CancellationToken);
                await using var memoryStream = new MemoryStream();
                await sourceStream.CopyToAsync(memoryStream, executeContext.CancellationToken);
                var sourceBytes = memoryStream.ToArray();

                await UploadResizedPictureAsync(sourceBytes, executeContext.Arguments.SmallDestinationKey, executeContext.Arguments.SmallSize, uploadedKeys, executeContext.CancellationToken);
                await UploadResizedPictureAsync(sourceBytes, executeContext.Arguments.MediumDestinationKey, executeContext.Arguments.MediumSize, uploadedKeys, executeContext.CancellationToken);
                await UploadResizedPictureAsync(sourceBytes, executeContext.Arguments.LargeDestinationKey, executeContext.Arguments.LargeSize, uploadedKeys, executeContext.CancellationToken);

                logger.LogInformation("Achievement picture processing completed for achievement {AchievementId}", executeContext.Arguments.AchievementId);
                return executeContext.Completed(new ProcessAchievementPictureLog(
                    executeContext.Arguments.SmallDestinationKey,
                    executeContext.Arguments.MediumDestinationKey,
                    executeContext.Arguments.LargeDestinationKey));
            }
            catch (Exception exception)
            {
                foreach (var uploadedKey in uploadedKeys)
                {
                    try
                    {
                        await s3Service.RemoveFileAsync(uploadedKey, executeContext.CancellationToken);
                    }
                    catch (Exception cleanupException)
                    {
                        logger.LogError(cleanupException, "Failed to clean up uploaded achievement picture {UploadedKey}", uploadedKey);
                    }
                }

                logger.LogError(exception, "An error occurred while processing achievement picture {SourceKey} for achievement {AchievementId}",
                    sourceKey,
                    executeContext.Arguments.AchievementId);
                throw;
            }
        }

        private async Task UploadResizedPictureAsync(
            byte[] sourceBytes,
            string destinationKey,
            PictureResizeSize size,
            ICollection<string> uploadedKeys,
            CancellationToken cancellationToken)
        {
            await using var resizedPicture = await ResizePictureAsync(sourceBytes, size, cancellationToken);
            await s3Service.UploadFileAsync(resizedPicture, destinationKey, "image/webp", cancellationToken);
            uploadedKeys.Add(destinationKey);
        }

        private async Task<Stream> ResizePictureAsync(byte[] sourceBytes, PictureResizeSize size, CancellationToken cancellationToken)
        {
            await using var sourceStream = new MemoryStream(sourceBytes, writable: false);
            return await pictureService.ResizePictureAsWebpAsync(sourceStream, new Size(size.Width, size.Height), cancellationToken);
        }

        private async Task RemoveFileIfExistsAsync(string keyName, CancellationToken cancellationToken)
        {
            await s3Service.RemoveFileAsync(keyName, cancellationToken);
        }
    }
}
