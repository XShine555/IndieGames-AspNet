using Application.Abstractions.Messaging.Games.V1;
using Infrastructure.Messaging.Configuration;
using Infrastructure.Messaging.Features.Common.Activities.Files;
using Infrastructure.Messaging.Features.Common.Activities.Pictures;
using Infrastructure.Messaging.Features.Games.Activities;
using Infrastructure.Messaging.Features.Games.Workflows.PictureProcessing.Arguments;
using Infrastructure.Messaging.Features.Games.Workflows.PictureProcessing.Variables;
using Infrastructure.Messaging.Helpers;
using MassTransit;

namespace Infrastructure.Messaging.Features.Games.Workflows.PictureProcessing.Builders
{
    public class PictureWorkflowRoutingSlipBuilder(WorkerConfiguration workerConfiguration)
    {
        public RoutingSlipBuilder Build(GenerateGamesPicturesEvent @event)
        {
            ArgumentNullException.ThrowIfNull(@event);
            var builder = new RoutingSlipBuilder(NewId.NextGuid());

            builder.AddActivity(
                GameActivityNames.GeneratePictureWorkflowPaths,
                EndpointHelper.BuildExecuteActivityUri(GeneratePictureWorkflowPathsActivity.ExecuteEndpointName),
                new GeneratePictureWorkflowPathsArguments(
                    workerConfiguration.Routes.TemporaryFilesDirectory,
                    @event.SourceKey));

            builder.AddActivity(
                GameActivityNames.DownloadFile,
                EndpointHelper.BuildExecuteActivityUri(DownloadFileFromBucketActivity.ExecuteEndpointName),
                new DownloadFileFromBucketArguments(
                    @event.SourceKey,
                    RoutingSlipVariableNames.Picture.OriginalFilePath
                    ));

            builder.AddActivity(
                GameActivityNames.ResizeSmall,
                EndpointHelper.BuildExecuteActivityUri(ResizePictureActivity.ExecuteEndpointName),
                new ResizePictureLocalArguments(
                    RoutingSlipVariableNames.Picture.OriginalFilePath,
                    RoutingSlipVariableNames.Picture.SmallResizedFilePath,
                    @event.SmallSize.Width,
                    @event.SmallSize.Height));

            builder.AddActivity(
                GameActivityNames.ResizeMedium,
                EndpointHelper.BuildExecuteActivityUri(ResizePictureActivity.ExecuteEndpointName),
                new ResizePictureLocalArguments(
                    RoutingSlipVariableNames.Picture.OriginalFilePath,
                    RoutingSlipVariableNames.Picture.MediumResizedFilePath,
                    @event.MediumSize.Width,
                    @event.MediumSize.Height));

            builder.AddActivity(
                GameActivityNames.ResizeLarge,
                EndpointHelper.BuildExecuteActivityUri(ResizePictureActivity.ExecuteEndpointName),
                new ResizePictureLocalArguments(
                    RoutingSlipVariableNames.Picture.OriginalFilePath,
                    RoutingSlipVariableNames.Picture.LargeResizedFilePath,
                    @event.LargeSize.Width,
                    @event.LargeSize.Height));

            builder.AddActivity(
                GameActivityNames.UploadSmall,
                EndpointHelper.BuildExecuteActivityUri(UploadFileToBucketActivity.ExecuteEndpointName),
                new UploadFileToBucketArguments(
                    RoutingSlipVariableNames.Picture.SmallResizedFilePath,
                    @event.SmallDestinationRoute));

            builder.AddActivity(
                GameActivityNames.UploadMedium,
                EndpointHelper.BuildExecuteActivityUri(UploadFileToBucketActivity.ExecuteEndpointName),
                new UploadFileToBucketArguments(
                    RoutingSlipVariableNames.Picture.MediumResizedFilePath,
                    @event.MediumDestinationRoute));

            builder.AddActivity(
                GameActivityNames.UploadLarge,
                EndpointHelper.BuildExecuteActivityUri(UploadFileToBucketActivity.ExecuteEndpointName),
                new UploadFileToBucketArguments(
                    RoutingSlipVariableNames.Picture.LargeResizedFilePath,
                    @event.LargeDestinationRoute));

            builder.AddActivity(
                GameActivityNames.SynchronizeGamePictures,
                EndpointHelper.BuildExecuteActivityUri(SynchronizeGamePicturesActivity.ExecuteEndpointName),
                new SynchronizeGamePicturesArguments(
                    @event.PictureId,
                    RoutingSlipVariableNames.Picture.SmallResizedFilePath,
                    RoutingSlipVariableNames.Picture.MediumResizedFilePath,
                    RoutingSlipVariableNames.Picture.LargeResizedFilePath));

            return builder;
        }
    }
}
