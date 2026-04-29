using Application.Abstractions.Messaging.Achievements.V1;
using Infrastructure.Messaging.Consumers;
using Infrastructure.Messaging.Features.Achievements.Activities;
using Infrastructure.Messaging.Helpers;
using MassTransit;
using MassTransit.Courier.Contracts;

namespace Infrastructure.Messaging.Features.Achievements.Workflows.AchievementPictureProcessing.Builders
{
    public class AchievementPictureWorkflowRoutingSlipBuilder
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
                ProcessAchievementPictureActivity.ExecuteEndpointName,
                EndpointHelper.BuildExecuteActivityUri(ProcessAchievementPictureActivity.ExecuteEndpointName),
                new ProcessAchievementPictureArguments(
                    @event.AchievementId,
                    @event.SourceKey,
                    @event.SmallDestinationKey,
                    @event.MediumDestinationKey,
                    @event.LargeDestinationKey,
                    @event.SmallSize,
                    @event.MediumSize,
                    @event.LargeSize));

            return builder;
        }
    }
}
