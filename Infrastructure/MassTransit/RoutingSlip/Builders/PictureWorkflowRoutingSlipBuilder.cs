using Application.Events;
using Infrastructure.Configurations;
using Infrastructure.MassTransit.Activities.Files;
using Infrastructure.MassTransit.Activities.Pictures;
using Infrastructure.MassTransit.Arguments;
using Infrastructure.MassTransit.Helpers;
using Infrastructure.MassTransit.RoutingSlip.Variables;
using MassTransit;

namespace Infrastructure.MassTransit.RoutingSlip.Builders
{
    public class PictureWorkflowRoutingSlipBuilder(WorkerConfiguration workerConfiguration)
    {
        public RoutingSlipBuilder Build(GenerateGamesPicturesEvent @event)
        {
            ArgumentNullException.ThrowIfNull(@event);
            var builder = new RoutingSlipBuilder(NewId.NextGuid());

            builder.AddActivity(
                ActivityNames.GeneratePictureWorkflowPaths,
                EndpointHelper.BuildExecuteActivityUri(GeneratePictureWorkflowPathsActivity.ExecuteEndpointName),
                new GeneratePictureWorkflowPathsArguments(
                    workerConfiguration.Routes.TemporaryFilesDirectory,
                    @event.SourceKey));

            builder.AddActivity(
                ActivityNames.ResizeSmall,
                EndpointHelper.BuildExecuteActivityUri(ResizePictureActivity.ExecuteEndpointName),
                new ResizePictureLocalArguments(
                    RoutingSlipVariableNames.Picture.OriginalFilePath,
                    RoutingSlipVariableNames.Picture.SmallResizedFilePath,
                    @event.SmallSize.Width,
                    @event.SmallSize.Height));

            builder.AddActivity(
                ActivityNames.ResizeMedium,
                EndpointHelper.BuildExecuteActivityUri(ResizePictureActivity.ExecuteEndpointName),
                new ResizePictureLocalArguments(
                    RoutingSlipVariableNames.Picture.OriginalFilePath,
                    RoutingSlipVariableNames.Picture.MediumResizedFilePath,
                    @event.MediumSize.Width,
                    @event.MediumSize.Height));

            builder.AddActivity(
                ActivityNames.ResizeLarge,
                EndpointHelper.BuildExecuteActivityUri(ResizePictureActivity.ExecuteEndpointName),
                new ResizePictureLocalArguments(
                    RoutingSlipVariableNames.Picture.OriginalFilePath,
                    RoutingSlipVariableNames.Picture.LargeResizedFilePath,
                    @event.LargeSize.Width,
                    @event.LargeSize.Height));

            builder.AddActivity(
                ActivityNames.UploadSmall,
                EndpointHelper.BuildExecuteActivityUri(UploadFileToBucketActivity.ExecuteEndpointName),
                new UploadFileToBucketArguments(
                    RoutingSlipVariableNames.Picture.SmallResizedFilePath,
                    @event.SmallDestinationRoute));

            builder.AddActivity(
                ActivityNames.UploadMedium,
                EndpointHelper.BuildExecuteActivityUri(UploadFileToBucketActivity.ExecuteEndpointName),
                new UploadFileToBucketArguments(
                    RoutingSlipVariableNames.Picture.MediumResizedFilePath,
                    @event.MediumDestinationRoute));

            builder.AddActivity(
                ActivityNames.UploadLarge,
                EndpointHelper.BuildExecuteActivityUri(UploadFileToBucketActivity.ExecuteEndpointName),
                new UploadFileToBucketArguments(
                    RoutingSlipVariableNames.Picture.LargeResizedFilePath,
                    @event.LargeDestinationRoute));

            builder.AddActivity(
                ActivityNames.SynchronizeGamePictures,
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