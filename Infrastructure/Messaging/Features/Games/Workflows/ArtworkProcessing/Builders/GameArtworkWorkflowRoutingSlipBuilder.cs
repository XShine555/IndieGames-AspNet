using Application.Abstractions.Messaging.Games.V1;
using Infrastructure.Messaging.Configuration;
using Infrastructure.Messaging.Features.Common.Activities.Files;
using Infrastructure.Messaging.Features.Common.Activities.Pictures;
using Infrastructure.Messaging.Features.Games.Activities;
using Infrastructure.Messaging.Features.Games.Workflows.ArtworkProcessing.Arguments;
using Infrastructure.Messaging.Features.Games.Workflows.ArtworkProcessing.Variables;
using Infrastructure.Messaging.Features.Games.Workflows.StorePictureProcessing.Arguments;
using Infrastructure.Messaging.Helpers;
using MassTransit;

namespace Infrastructure.Messaging.Features.Games.Workflows.ArtworkProcessing.Builders
{
    public class GameArtworkWorkflowRoutingSlipBuilder(WorkerConfiguration workerConfiguration)
    {
        public RoutingSlipBuilder Build(ProcessNewGameArtworkEvent @event)
        {
            ArgumentNullException.ThrowIfNull(@event);
            var builder = new RoutingSlipBuilder(NewId.NextGuid());

            builder.AddActivity(
                GameArtworkActivityNames.GeneratePictureWorkflowPaths,
                EndpointHelper.BuildExecuteActivityUri(GenerateGameArtworkWorkflowPathsActivity.ExecuteEndpointName),
                new GenerateGameArtworkWorkflowPathsArguments(
                    workerConfiguration.Routes.TemporaryFilesDirectory,
                    @event.SourceKey));

            builder.AddActivity(
                GameArtworkActivityNames.DownloadFile,
                EndpointHelper.BuildExecuteActivityUri(DownloadFileFromBucketActivity.ExecuteEndpointName),
                new DownloadFileFromBucketArguments(
                    @event.SourceKey,
                    GameArtworkRoutingSlipVariableNames.Picture.OriginalFilePath));

            builder.AddActivity(
                GameArtworkActivityNames.ResizeSmall,
                EndpointHelper.BuildExecuteActivityUri(ResizePictureActivity.ExecuteEndpointName),
                new ResizePictureLocalArguments(
                    GameArtworkRoutingSlipVariableNames.Picture.OriginalFilePath,
                    GameArtworkRoutingSlipVariableNames.Picture.SmallResizedFilePath,
                    @event.SmallSize.Width,
                    @event.SmallSize.Height));

            builder.AddActivity(
                GameArtworkActivityNames.ResizeMedium,
                EndpointHelper.BuildExecuteActivityUri(ResizePictureActivity.ExecuteEndpointName),
                new ResizePictureLocalArguments(
                    GameArtworkRoutingSlipVariableNames.Picture.OriginalFilePath,
                    GameArtworkRoutingSlipVariableNames.Picture.MediumResizedFilePath,
                    @event.MediumSize.Width,
                    @event.MediumSize.Height));

            builder.AddActivity(
                GameArtworkActivityNames.ResizeLarge,
                EndpointHelper.BuildExecuteActivityUri(ResizePictureActivity.ExecuteEndpointName),
                new ResizePictureLocalArguments(
                    GameArtworkRoutingSlipVariableNames.Picture.OriginalFilePath,
                    GameArtworkRoutingSlipVariableNames.Picture.LargeResizedFilePath,
                    @event.LargeSize.Width,
                    @event.LargeSize.Height));

            builder.AddActivity(
                GameArtworkActivityNames.UploadSmall,
                EndpointHelper.BuildExecuteActivityUri(UploadFileToBucketActivity.ExecuteEndpointName),
                new UploadFileToBucketArguments(
                    GameArtworkRoutingSlipVariableNames.Picture.SmallResizedFilePath,
                    @event.SmallDestinationRoute));

            builder.AddActivity(
                GameArtworkActivityNames.UploadMedium,
                EndpointHelper.BuildExecuteActivityUri(UploadFileToBucketActivity.ExecuteEndpointName),
                new UploadFileToBucketArguments(
                    GameArtworkRoutingSlipVariableNames.Picture.MediumResizedFilePath,
                    @event.MediumDestinationRoute));

            builder.AddActivity(
                GameArtworkActivityNames.UploadLarge,
                EndpointHelper.BuildExecuteActivityUri(UploadFileToBucketActivity.ExecuteEndpointName),
                new UploadFileToBucketArguments(
                    GameArtworkRoutingSlipVariableNames.Picture.LargeResizedFilePath,
                    @event.LargeDestinationRoute));

            builder.AddActivity(
                GameArtworkActivityNames.SynchronizeGameArtwork,
                EndpointHelper.BuildExecuteActivityUri(SynchronizeGameArtworkActivity.ExecuteEndpointName),
                new SynchronizeGameArtworkArguments(
                    @event.ArtworkId,
                    GameArtworkRoutingSlipVariableNames.Picture.SmallResizedFilePath,
                    GameArtworkRoutingSlipVariableNames.Picture.MediumResizedFilePath,
                    GameArtworkRoutingSlipVariableNames.Picture.LargeResizedFilePath,
                    @event.SmallDestinationRoute,
                    @event.MediumDestinationRoute,
                    @event.LargeDestinationRoute,
                    @event.SmallSize.Width,
                    @event.SmallSize.Height,
                    @event.MediumSize.Width,
                    @event.MediumSize.Height,
                    @event.LargeSize.Width,
                    @event.LargeSize.Height));

            return builder;
        }
    }
}
