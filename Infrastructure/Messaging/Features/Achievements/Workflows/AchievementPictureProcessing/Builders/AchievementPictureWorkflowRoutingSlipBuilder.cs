using Application.Abstractions.Messaging.Achievements.V1;
using Infrastructure.Messaging.Configuration;
using Infrastructure.Messaging.Consumers;
using Infrastructure.Messaging.Features.Common.Activities.Files;
using Infrastructure.Messaging.Features.Common.Activities.Pictures;
using Infrastructure.Messaging.Features.Achievements.Activities;
using Infrastructure.Messaging.Features.Achievements.Workflows.AchievementPictureProcessing.Arguments;
using Infrastructure.Messaging.Features.Achievements.Workflows.AchievementPictureProcessing.Variables;
using Infrastructure.Messaging.Features.Games.Workflows.StorePictureProcessing.Arguments;
using Infrastructure.Messaging.Helpers;
using MassTransit;
using MassTransit.Courier.Contracts;

namespace Infrastructure.Messaging.Features.Achievements.Workflows.AchievementPictureProcessing.Builders
{
    public class AchievementPictureWorkflowRoutingSlipBuilder(WorkerConfiguration workerConfiguration)
    {
        public const string ProcessName = "GenerateAchievementsPictures";

        public RoutingSlipBuilder Build(GenerateAchievementsPicturesEvent @event)
        {
            ArgumentNullException.ThrowIfNull(@event);
            var builder = new RoutingSlipBuilder(NewId.NextGuid());
            builder.AddSubscription(
                EndpointHelper.BuildConsumerUri(RoutingSlipCleanUpConsumer.QueueName),
                RoutingSlipEvents.Completed | RoutingSlipEvents.Faulted);

            builder.AddActivity(
                AchievementPictureActivityNames.GeneratePictureWorkflowPaths,
                EndpointHelper.BuildExecuteActivityUri(GenerateAchievementPictureWorkflowPathsActivity.ExecuteEndpointName),
                new GenerateAchievementPictureWorkflowPathsArguments(
                    workerConfiguration.Routes.TemporaryFilesDirectory,
                    @event.SourceKey));

            builder.AddActivity(
                AchievementPictureActivityNames.DownloadFile,
                EndpointHelper.BuildExecuteActivityUri(DownloadFileFromBucketActivity.ExecuteEndpointName),
                new DownloadFileFromBucketArguments(
                    @event.SourceKey,
                    AchievementPictureRoutingSlipVariableNames.Picture.OriginalFilePath));

            builder.AddActivity(
                AchievementPictureActivityNames.ResizeSmall,
                EndpointHelper.BuildExecuteActivityUri(ResizePictureActivity.ExecuteEndpointName),
                new ResizePictureLocalArguments(
                    AchievementPictureRoutingSlipVariableNames.Picture.OriginalFilePath,
                    AchievementPictureRoutingSlipVariableNames.Picture.SmallResizedFilePath,
                    @event.SmallSize.Width,
                    @event.SmallSize.Height));

            builder.AddActivity(
                AchievementPictureActivityNames.ResizeMedium,
                EndpointHelper.BuildExecuteActivityUri(ResizePictureActivity.ExecuteEndpointName),
                new ResizePictureLocalArguments(
                    AchievementPictureRoutingSlipVariableNames.Picture.OriginalFilePath,
                    AchievementPictureRoutingSlipVariableNames.Picture.MediumResizedFilePath,
                    @event.MediumSize.Width,
                    @event.MediumSize.Height));

            builder.AddActivity(
                AchievementPictureActivityNames.ResizeLarge,
                EndpointHelper.BuildExecuteActivityUri(ResizePictureActivity.ExecuteEndpointName),
                new ResizePictureLocalArguments(
                    AchievementPictureRoutingSlipVariableNames.Picture.OriginalFilePath,
                    AchievementPictureRoutingSlipVariableNames.Picture.LargeResizedFilePath,
                    @event.LargeSize.Width,
                    @event.LargeSize.Height));

            builder.AddActivity(
                AchievementPictureActivityNames.UploadSmall,
                EndpointHelper.BuildExecuteActivityUri(UploadFileToBucketActivity.ExecuteEndpointName),
                new UploadFileToBucketArguments(
                    AchievementPictureRoutingSlipVariableNames.Picture.SmallResizedFilePath,
                    @event.SmallDestinationKey));

            builder.AddActivity(
                AchievementPictureActivityNames.UploadMedium,
                EndpointHelper.BuildExecuteActivityUri(UploadFileToBucketActivity.ExecuteEndpointName),
                new UploadFileToBucketArguments(
                    AchievementPictureRoutingSlipVariableNames.Picture.MediumResizedFilePath,
                    @event.MediumDestinationKey));

            builder.AddActivity(
                AchievementPictureActivityNames.UploadLarge,
                EndpointHelper.BuildExecuteActivityUri(UploadFileToBucketActivity.ExecuteEndpointName),
                new UploadFileToBucketArguments(
                    AchievementPictureRoutingSlipVariableNames.Picture.LargeResizedFilePath,
                    @event.LargeDestinationKey));

            builder.AddActivity(
                AchievementPictureActivityNames.SynchronizeAchievementPictures,
                EndpointHelper.BuildExecuteActivityUri(SynchronizeAchievementPicturesActivity.ExecuteEndpointName),
                new SynchronizeAchievementPicturesArguments(
                    @event.AchievementId,
                    AchievementPictureRoutingSlipVariableNames.Picture.SmallResizedFilePath,
                    AchievementPictureRoutingSlipVariableNames.Picture.MediumResizedFilePath,
                    AchievementPictureRoutingSlipVariableNames.Picture.LargeResizedFilePath,
                    @event.SmallDestinationKey,
                    @event.MediumDestinationKey,
                    @event.LargeDestinationKey));

            return builder;
        }
    }
}
