using Application.Abstractions.Messaging.Games.V1;
using Application.Abstractions.Persistence;
using Domain.Entities;
using Infrastructure.Messaging.Configuration;
using Infrastructure.Messaging.Consumers;
using Infrastructure.Messaging.Features.Common.Activities.Files;
using Infrastructure.Messaging.Features.Common.Activities.Pictures;
using Infrastructure.Messaging.Features.Games.Activities;
using Infrastructure.Messaging.Features.Games.Workflows.ArtworkProcessing.Arguments;
using Infrastructure.Messaging.Features.Games.Workflows.ArtworkProcessing.Variables;
using Infrastructure.Messaging.Features.Games.Workflows.StorePictureProcessing.Arguments;
using Infrastructure.Messaging.Helpers;
using MassTransit;
using MassTransit.Courier.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Messaging.Features.Games.Workflows.ArtworkProcessing.Builders
{
    public class GameArtworkWorkflowRoutingSlipBuilder(WorkerConfiguration workerConfiguration,
        IDatabase database)
    {
        public const string ProcessName = "ProcessNewGameArtwork";

        public RoutingSlipBuilder Build(ProcessNewGameArtworkEvent @event)
        {
            ArgumentNullException.ThrowIfNull(@event);

            async Task MarkAsFailedAsync(CancellationToken cancellationToken)
            {
                var artwork = await database.GameArtworks.SingleAsync(a => a.Id == @event.ArtworkId, cancellationToken);
                artwork.ProcessingStatus = GameArtworkProcessingStatus.Failed;
                artwork.ProcessingError = "An error occurred during the processing of the game artwork. Please check the routing slip execution logs for more details.";
                database.GameArtworks.Update(artwork);
                await database.SaveChangesAsync(cancellationToken);
            }

            var builder = new RoutingSlipBuilder(NewId.NextGuid());
            builder.AddSubscription(
                EndpointHelper.BuildConsumerUri(RoutingSlipCleanUpConsumer.QueueName),
                RoutingSlipEvents.Completed | RoutingSlipEvents.Faulted);

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
                    GameArtworkRoutingSlipVariableNames.Picture.OriginalFilePath,
                    MarkAsFailedAsync));

            builder.AddActivity(
                GameArtworkActivityNames.ResizeSmall,
                EndpointHelper.BuildExecuteActivityUri(ResizePictureActivity.ExecuteEndpointName),
                new ResizePictureLocalArguments(
                    GameArtworkRoutingSlipVariableNames.Picture.OriginalFilePath,
                    GameArtworkRoutingSlipVariableNames.Picture.SmallResizedFilePath,
                    @event.SmallSize.Width,
                    @event.SmallSize.Height,
                    MarkAsFailedAsync));

            builder.AddActivity(
                GameArtworkActivityNames.ResizeMedium,
                EndpointHelper.BuildExecuteActivityUri(ResizePictureActivity.ExecuteEndpointName),
                new ResizePictureLocalArguments(
                    GameArtworkRoutingSlipVariableNames.Picture.OriginalFilePath,
                    GameArtworkRoutingSlipVariableNames.Picture.MediumResizedFilePath,
                    @event.MediumSize.Width,
                    @event.MediumSize.Height,
                    MarkAsFailedAsync));

            builder.AddActivity(
                GameArtworkActivityNames.ResizeLarge,
                EndpointHelper.BuildExecuteActivityUri(ResizePictureActivity.ExecuteEndpointName),
                new ResizePictureLocalArguments(
                    GameArtworkRoutingSlipVariableNames.Picture.OriginalFilePath,
                    GameArtworkRoutingSlipVariableNames.Picture.LargeResizedFilePath,
                    @event.LargeSize.Width,
                    @event.LargeSize.Height,
                    MarkAsFailedAsync));

            builder.AddActivity(
                GameArtworkActivityNames.UploadSmall,
                EndpointHelper.BuildExecuteActivityUri(UploadFileToBucketActivity.ExecuteEndpointName),
                new UploadFileToBucketArguments(
                    GameArtworkRoutingSlipVariableNames.Picture.SmallResizedFilePath,
                    @event.SmallDestinationRoute,
                    MarkAsFailedAsync));

            builder.AddActivity(
                GameArtworkActivityNames.UploadMedium,
                EndpointHelper.BuildExecuteActivityUri(UploadFileToBucketActivity.ExecuteEndpointName),
                new UploadFileToBucketArguments(
                    GameArtworkRoutingSlipVariableNames.Picture.MediumResizedFilePath,
                    @event.MediumDestinationRoute,
                    MarkAsFailedAsync));

            builder.AddActivity(
                GameArtworkActivityNames.UploadLarge,
                EndpointHelper.BuildExecuteActivityUri(UploadFileToBucketActivity.ExecuteEndpointName),
                new UploadFileToBucketArguments(
                    GameArtworkRoutingSlipVariableNames.Picture.LargeResizedFilePath,
                    @event.LargeDestinationRoute,
                    MarkAsFailedAsync));

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
