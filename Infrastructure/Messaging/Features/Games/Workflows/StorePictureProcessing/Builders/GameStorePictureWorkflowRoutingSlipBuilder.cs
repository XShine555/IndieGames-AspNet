using Application.Abstractions.Messaging.Games.V1;
using Application.Abstractions.Persistence;
using Domain.Entities;
using Infrastructure.Messaging.Configuration;
using Infrastructure.Messaging.Consumers;
using Infrastructure.Messaging.Features.Common.Activities.Files;
using Infrastructure.Messaging.Features.Common.Activities.Pictures;
using Infrastructure.Messaging.Features.Games.Activities;
using Infrastructure.Messaging.Features.Games.Workflows.StorePictureProcessing.Arguments;
using Infrastructure.Messaging.Features.Games.Workflows.StorePictureProcessing.Variables;
using Infrastructure.Messaging.Helpers;
using MassTransit;
using MassTransit.Courier.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Messaging.Features.Games.Workflows.StorePictureProcessing.Builders
{
    public class GameStorePictureWorkflowRoutingSlipBuilder(
        WorkerConfiguration workerConfiguration,
        IDatabase database)
    {
        public const string ProcessName = "GenerateGamesPictures";

        public RoutingSlipBuilder Build(GenerateGamesPicturesEvent @event)
        {
            ArgumentNullException.ThrowIfNull(@event);

            async Task MarkAsFailedAsync(CancellationToken cancellationToken)
            {
                var picture = await database.GamePictures
                    .SingleAsync(p => p.Id == @event.PictureId, cancellationToken);

                picture.ProcessingStatus = GamePictureProcessingStatus.Failed;
                database.GamePictures.Update(picture);
                await database.SaveChangesAsync(cancellationToken);
            }

            var builder = new RoutingSlipBuilder(NewId.NextGuid());
            builder.AddSubscription(
                EndpointHelper.BuildConsumerUri(RoutingSlipCleanUpConsumer.QueueName),
                RoutingSlipEvents.Completed | RoutingSlipEvents.Faulted);

            builder.AddActivity(
                GameStorePictureActivityNames.GeneratePictureWorkflowPaths,
                EndpointHelper.BuildExecuteActivityUri(GenerateGameStorePictureWorkflowPathsActivity.ExecuteEndpointName),
                new GenerateGameStorePictureWorkflowPathsArguments(
                    workerConfiguration.Routes.TemporaryFilesDirectory,
                    @event.SourceKey));

            builder.AddActivity(
                GameStorePictureActivityNames.DownloadFile,
                EndpointHelper.BuildExecuteActivityUri(DownloadFileFromBucketActivity.ExecuteEndpointName),
                new DownloadFileFromBucketArguments(
                    @event.SourceKey,
                    GameStorePictureRoutingSlipVariableNames.Picture.OriginalFilePath,
                    MarkAsFailedAsync));

            builder.AddActivity(
                GameStorePictureActivityNames.ResizeSmall,
                EndpointHelper.BuildExecuteActivityUri(ResizePictureActivity.ExecuteEndpointName),
                new ResizePictureLocalArguments(
                    GameStorePictureRoutingSlipVariableNames.Picture.OriginalFilePath,
                    GameStorePictureRoutingSlipVariableNames.Picture.SmallResizedFilePath,
                    @event.SmallSize.Width,
                    @event.SmallSize.Height,
                    MarkAsFailedAsync));

            builder.AddActivity(
                GameStorePictureActivityNames.ResizeMedium,
                EndpointHelper.BuildExecuteActivityUri(ResizePictureActivity.ExecuteEndpointName),
                new ResizePictureLocalArguments(
                    GameStorePictureRoutingSlipVariableNames.Picture.OriginalFilePath,
                    GameStorePictureRoutingSlipVariableNames.Picture.MediumResizedFilePath,
                    @event.MediumSize.Width,
                    @event.MediumSize.Height,
                    MarkAsFailedAsync));

            builder.AddActivity(
                GameStorePictureActivityNames.ResizeLarge,
                EndpointHelper.BuildExecuteActivityUri(ResizePictureActivity.ExecuteEndpointName),
                new ResizePictureLocalArguments(
                    GameStorePictureRoutingSlipVariableNames.Picture.OriginalFilePath,
                    GameStorePictureRoutingSlipVariableNames.Picture.LargeResizedFilePath,
                    @event.LargeSize.Width,
                    @event.LargeSize.Height,
                    MarkAsFailedAsync));

            builder.AddActivity(
                GameStorePictureActivityNames.UploadSmall,
                EndpointHelper.BuildExecuteActivityUri(UploadFileToBucketActivity.ExecuteEndpointName),
                new UploadFileToBucketArguments(
                    GameStorePictureRoutingSlipVariableNames.Picture.SmallResizedFilePath,
                    @event.SmallDestinationRoute,
                    MarkAsFailedAsync));

            builder.AddActivity(
                GameStorePictureActivityNames.UploadMedium,
                EndpointHelper.BuildExecuteActivityUri(UploadFileToBucketActivity.ExecuteEndpointName),
                new UploadFileToBucketArguments(
                    GameStorePictureRoutingSlipVariableNames.Picture.MediumResizedFilePath,
                    @event.MediumDestinationRoute,
                    MarkAsFailedAsync));

            builder.AddActivity(
                GameStorePictureActivityNames.UploadLarge,
                EndpointHelper.BuildExecuteActivityUri(UploadFileToBucketActivity.ExecuteEndpointName),
                new UploadFileToBucketArguments(
                    GameStorePictureRoutingSlipVariableNames.Picture.LargeResizedFilePath,
                    @event.LargeDestinationRoute,
                    MarkAsFailedAsync));

            builder.AddActivity(
                GameStorePictureActivityNames.SynchronizeGameStorePictures,
                EndpointHelper.BuildExecuteActivityUri(SynchronizeGameStorePicturesActivity.ExecuteEndpointName),
                new SynchronizeGameStorePicturesArguments(
                    @event.PictureId,
                    GameStorePictureRoutingSlipVariableNames.Picture.SmallResizedFilePath,
                    GameStorePictureRoutingSlipVariableNames.Picture.MediumResizedFilePath,
                    GameStorePictureRoutingSlipVariableNames.Picture.LargeResizedFilePath,
                    @event.SmallDestinationRoute,
                    @event.MediumDestinationRoute,
                    @event.LargeDestinationRoute));

            return builder;
        }
    }
}
