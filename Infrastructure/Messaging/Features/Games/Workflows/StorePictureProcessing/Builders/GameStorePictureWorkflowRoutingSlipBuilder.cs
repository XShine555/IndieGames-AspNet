using Application.Abstractions.Messaging.Games.V1;
using Infrastructure.Messaging.Configuration;
using Infrastructure.Messaging.Features.Common.Activities.Files;
using Infrastructure.Messaging.Features.Common.Activities.Pictures;
using Infrastructure.Messaging.Features.Games.Activities;
using Infrastructure.Messaging.Features.Games.Workflows.StorePictureProcessing.Arguments;
using Infrastructure.Messaging.Features.Games.Workflows.StorePictureProcessing.Variables;
using Infrastructure.Messaging.Helpers;
using MassTransit;

namespace Infrastructure.Messaging.Features.Games.Workflows.StorePictureProcessing.Builders
{
    public class GameStorePictureWorkflowRoutingSlipBuilder(WorkerConfiguration workerConfiguration)
    {
        public RoutingSlipBuilder Build(GenerateGamesPicturesEvent @event)
        {
            ArgumentNullException.ThrowIfNull(@event);
            var builder = new RoutingSlipBuilder(NewId.NextGuid());

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
                    GameStorePictureRoutingSlipVariableNames.Picture.OriginalFilePath));

            builder.AddActivity(
                GameStorePictureActivityNames.ResizeSmall,
                EndpointHelper.BuildExecuteActivityUri(ResizePictureActivity.ExecuteEndpointName),
                new ResizePictureLocalArguments(
                    GameStorePictureRoutingSlipVariableNames.Picture.OriginalFilePath,
                    GameStorePictureRoutingSlipVariableNames.Picture.SmallResizedFilePath,
                    @event.SmallSize.Width,
                    @event.SmallSize.Height));

            builder.AddActivity(
                GameStorePictureActivityNames.ResizeMedium,
                EndpointHelper.BuildExecuteActivityUri(ResizePictureActivity.ExecuteEndpointName),
                new ResizePictureLocalArguments(
                    GameStorePictureRoutingSlipVariableNames.Picture.OriginalFilePath,
                    GameStorePictureRoutingSlipVariableNames.Picture.MediumResizedFilePath,
                    @event.MediumSize.Width,
                    @event.MediumSize.Height));

            builder.AddActivity(
                GameStorePictureActivityNames.ResizeLarge,
                EndpointHelper.BuildExecuteActivityUri(ResizePictureActivity.ExecuteEndpointName),
                new ResizePictureLocalArguments(
                    GameStorePictureRoutingSlipVariableNames.Picture.OriginalFilePath,
                    GameStorePictureRoutingSlipVariableNames.Picture.LargeResizedFilePath,
                    @event.LargeSize.Width,
                    @event.LargeSize.Height));

            builder.AddActivity(
                GameStorePictureActivityNames.UploadSmall,
                EndpointHelper.BuildExecuteActivityUri(UploadFileToBucketActivity.ExecuteEndpointName),
                new UploadFileToBucketArguments(
                    GameStorePictureRoutingSlipVariableNames.Picture.SmallResizedFilePath,
                    @event.SmallDestinationRoute));

            builder.AddActivity(
                GameStorePictureActivityNames.UploadMedium,
                EndpointHelper.BuildExecuteActivityUri(UploadFileToBucketActivity.ExecuteEndpointName),
                new UploadFileToBucketArguments(
                    GameStorePictureRoutingSlipVariableNames.Picture.MediumResizedFilePath,
                    @event.MediumDestinationRoute));

            builder.AddActivity(
                GameStorePictureActivityNames.UploadLarge,
                EndpointHelper.BuildExecuteActivityUri(UploadFileToBucketActivity.ExecuteEndpointName),
                new UploadFileToBucketArguments(
                    GameStorePictureRoutingSlipVariableNames.Picture.LargeResizedFilePath,
                    @event.LargeDestinationRoute));

            builder.AddActivity(
                GameStorePictureActivityNames.SynchronizeGameStorePictures,
                EndpointHelper.BuildExecuteActivityUri(SynchronizeGameStorePicturesActivity.ExecuteEndpointName),
                new SynchronizeGameStorePicturesArguments(
                    @event.PictureId,
                    GameStorePictureRoutingSlipVariableNames.Picture.SmallResizedFilePath,
                    GameStorePictureRoutingSlipVariableNames.Picture.MediumResizedFilePath,
                    GameStorePictureRoutingSlipVariableNames.Picture.LargeResizedFilePath));

            return builder;
        }
    }
}
