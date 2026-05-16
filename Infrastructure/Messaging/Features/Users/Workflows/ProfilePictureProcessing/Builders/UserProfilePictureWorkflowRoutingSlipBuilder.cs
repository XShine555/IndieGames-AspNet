using Application.Abstractions.Messaging.Users.V1;
using Infrastructure.Messaging.Configuration;
using Infrastructure.Messaging.Consumers;
using Infrastructure.Messaging.Features.Common.Activities.Files;
using Infrastructure.Messaging.Features.Common.Activities.Pictures;
using Infrastructure.Messaging.Features.Games.Workflows.StorePictureProcessing.Arguments;
using Infrastructure.Messaging.Features.Users.Activities;
using Infrastructure.Messaging.Features.Users.Workflows.ProfilePictureProcessing.Arguments;
using Infrastructure.Messaging.Features.Users.Workflows.ProfilePictureProcessing.Variables;
using Infrastructure.Messaging.Helpers;
using MassTransit;
using MassTransit.Courier.Contracts;

namespace Infrastructure.Messaging.Features.Users.Workflows.ProfilePictureProcessing.Builders
{
    public class UserProfilePictureWorkflowRoutingSlipBuilder(WorkerConfiguration workerConfiguration)
    {
        public const string ProcessName = "GenerateUsersProfilePictures";

        public RoutingSlipBuilder Build(GenerateUsersProfilePicturesEvent @event)
        {
            ArgumentNullException.ThrowIfNull(@event);
            var builder = new RoutingSlipBuilder(NewId.NextGuid());
            builder.AddSubscription(
                EndpointHelper.BuildConsumerUri(RoutingSlipCleanUpConsumer.QueueName),
                RoutingSlipEvents.Completed | RoutingSlipEvents.Faulted);

            builder.AddActivity(
                UserProfilePictureActivityNames.GeneratePictureWorkflowPaths,
                EndpointHelper.BuildExecuteActivityUri(GenerateUserProfilePictureWorkflowPathsActivity.ExecuteEndpointName),
                new GeneratePictureWorkflowPathsArguments(
                    workerConfiguration.Routes.TemporaryFilesDirectory,
                    @event.SourceKey));

            builder.AddActivity(
                UserProfilePictureActivityNames.DownloadFile,
                EndpointHelper.BuildExecuteActivityUri(DownloadFileFromBucketActivity.ExecuteEndpointName),
                new DownloadFileFromBucketArguments(
                    @event.SourceKey,
                    UserProfilePictureRoutingSlipVariableNames.Picture.OriginalFilePath));

            builder.AddActivity(
                UserProfilePictureActivityNames.ResizeSmall,
                EndpointHelper.BuildExecuteActivityUri(ResizePictureActivity.ExecuteEndpointName),
                new ResizePictureLocalArguments(
                    UserProfilePictureRoutingSlipVariableNames.Picture.OriginalFilePath,
                    UserProfilePictureRoutingSlipVariableNames.Picture.SmallResizedFilePath,
                    @event.SmallSize.Width,
                    @event.SmallSize.Height));

            builder.AddActivity(
                UserProfilePictureActivityNames.ResizeMedium,
                EndpointHelper.BuildExecuteActivityUri(ResizePictureActivity.ExecuteEndpointName),
                new ResizePictureLocalArguments(
                    UserProfilePictureRoutingSlipVariableNames.Picture.OriginalFilePath,
                    UserProfilePictureRoutingSlipVariableNames.Picture.MediumResizedFilePath,
                    @event.MediumSize.Width,
                    @event.MediumSize.Height));

            builder.AddActivity(
                UserProfilePictureActivityNames.ResizeLarge,
                EndpointHelper.BuildExecuteActivityUri(ResizePictureActivity.ExecuteEndpointName),
                new ResizePictureLocalArguments(
                    UserProfilePictureRoutingSlipVariableNames.Picture.OriginalFilePath,
                    UserProfilePictureRoutingSlipVariableNames.Picture.LargeResizedFilePath,
                    @event.LargeSize.Width,
                    @event.LargeSize.Height));

            builder.AddActivity(
                UserProfilePictureActivityNames.UploadSmall,
                EndpointHelper.BuildExecuteActivityUri(UploadFileToBucketActivity.ExecuteEndpointName),
                new UploadFileToBucketArguments(
                    UserProfilePictureRoutingSlipVariableNames.Picture.SmallResizedFilePath,
                    @event.SmallDestinationRoute));

            builder.AddActivity(
                UserProfilePictureActivityNames.UploadMedium,
                EndpointHelper.BuildExecuteActivityUri(UploadFileToBucketActivity.ExecuteEndpointName),
                new UploadFileToBucketArguments(
                    UserProfilePictureRoutingSlipVariableNames.Picture.MediumResizedFilePath,
                    @event.MediumDestinationRoute));

            builder.AddActivity(
                UserProfilePictureActivityNames.UploadLarge,
                EndpointHelper.BuildExecuteActivityUri(UploadFileToBucketActivity.ExecuteEndpointName),
                new UploadFileToBucketArguments(
                    UserProfilePictureRoutingSlipVariableNames.Picture.LargeResizedFilePath,
                    @event.LargeDestinationRoute));

            builder.AddActivity(
                UserProfilePictureActivityNames.SynchronizeUserProfilePictures,
                EndpointHelper.BuildExecuteActivityUri(SynchronizeUserProfilePicturesActivity.ExecuteEndpointName),
                new SynchronizeUserProfilePicturesArguments(
                    @event.PictureId,
                    UserProfilePictureRoutingSlipVariableNames.Picture.SmallResizedFilePath,
                    UserProfilePictureRoutingSlipVariableNames.Picture.MediumResizedFilePath,
                    UserProfilePictureRoutingSlipVariableNames.Picture.LargeResizedFilePath,
                    @event.SmallDestinationRoute,
                    @event.MediumDestinationRoute,
                    @event.LargeDestinationRoute));

            return builder;
        }
    }
}
