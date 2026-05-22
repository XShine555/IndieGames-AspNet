using Application.Abstractions.Messaging.Games.V1;
using Infrastructure.Messaging.Configuration;
using Infrastructure.Messaging.Consumers;
using Infrastructure.Messaging.Features.Common.Activities.Files;
using Infrastructure.Messaging.Features.Common.Activities.Pictures;
using Infrastructure.Messaging.Features.Games.Activities;
using Infrastructure.Messaging.Features.Common.Workflows;
using Infrastructure.Messaging.Features.Games.Workflows.StorePictureProcessing.Arguments;
using Infrastructure.Messaging.Features.Games.Workflows.StorePictureProcessing.Variables;
using Infrastructure.Messaging.Helpers;
using MassTransit;
using MassTransit.Courier.Contracts;

namespace Infrastructure.Messaging.Features.Games.Workflows.StorePictureProcessing.Builders
{
    public class GameStorePictureWorkflowRoutingSlipBuilder(WorkerConfiguration workerConfiguration)
    {
        public const string ProcessName = "GenerateGamesPictures";

        public RoutingSlipBuilder Build(GenerateGamesPicturesEvent @event)
        {
            ArgumentNullException.ThrowIfNull(@event);

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
                    PictureProcessingWorkflowContextType.StorePictureProcessing,
                    @event.PictureId));

            builder.AddActivity(
                GameStorePictureActivityNames.ResizeSmall,
                EndpointHelper.BuildExecuteActivityUri(ResizePictureActivity.ExecuteEndpointName),
                new ResizePictureLocalArguments(
                    GameStorePictureRoutingSlipVariableNames.Picture.OriginalFilePath,
                    GameStorePictureRoutingSlipVariableNames.Picture.SmallResizedFilePath,
                    @event.SmallSize.Width,
                    @event.SmallSize.Height,
                    PictureProcessingWorkflowContextType.StorePictureProcessing,
                    @event.PictureId));

            builder.AddActivity(
                GameStorePictureActivityNames.ResizeMedium,
                EndpointHelper.BuildExecuteActivityUri(ResizePictureActivity.ExecuteEndpointName),
                new ResizePictureLocalArguments(
                    GameStorePictureRoutingSlipVariableNames.Picture.OriginalFilePath,
                    GameStorePictureRoutingSlipVariableNames.Picture.MediumResizedFilePath,
                    @event.MediumSize.Width,
                    @event.MediumSize.Height,
                    PictureProcessingWorkflowContextType.StorePictureProcessing,
                    @event.PictureId));

            builder.AddActivity(
                GameStorePictureActivityNames.ResizeLarge,
                EndpointHelper.BuildExecuteActivityUri(ResizePictureActivity.ExecuteEndpointName),
                new ResizePictureLocalArguments(
                    GameStorePictureRoutingSlipVariableNames.Picture.OriginalFilePath,
                    GameStorePictureRoutingSlipVariableNames.Picture.LargeResizedFilePath,
                    @event.LargeSize.Width,
                    @event.LargeSize.Height,
                    PictureProcessingWorkflowContextType.StorePictureProcessing,
                    @event.PictureId));

            builder.AddActivity(
                GameStorePictureActivityNames.UploadSmall,
                EndpointHelper.BuildExecuteActivityUri(UploadFileToBucketActivity.ExecuteEndpointName),
                new UploadFileToBucketArguments(
                    GameStorePictureRoutingSlipVariableNames.Picture.SmallResizedFilePath,
                    @event.SmallDestinationRoute,
                    PictureProcessingWorkflowContextType.StorePictureProcessing,
                    @event.PictureId));

            builder.AddActivity(
                GameStorePictureActivityNames.UploadMedium,
                EndpointHelper.BuildExecuteActivityUri(UploadFileToBucketActivity.ExecuteEndpointName),
                new UploadFileToBucketArguments(
                    GameStorePictureRoutingSlipVariableNames.Picture.MediumResizedFilePath,
                    @event.MediumDestinationRoute,
                    PictureProcessingWorkflowContextType.StorePictureProcessing,
                    @event.PictureId));

            builder.AddActivity(
                GameStorePictureActivityNames.UploadLarge,
                EndpointHelper.BuildExecuteActivityUri(UploadFileToBucketActivity.ExecuteEndpointName),
                new UploadFileToBucketArguments(
                    GameStorePictureRoutingSlipVariableNames.Picture.LargeResizedFilePath,
                    @event.LargeDestinationRoute,
                    PictureProcessingWorkflowContextType.StorePictureProcessing,
                    @event.PictureId));

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
