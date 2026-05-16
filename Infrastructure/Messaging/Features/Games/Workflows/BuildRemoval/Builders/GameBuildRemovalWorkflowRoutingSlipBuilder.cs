using Application.Abstractions.Messaging.Games.V1;
using Infrastructure.Messaging.Consumers;
using Infrastructure.Messaging.Features.Games.Activities;
using Infrastructure.Messaging.Features.Games.Workflows.BuildRemoval.Arguments;
using Infrastructure.Messaging.Helpers;
using MassTransit;
using MassTransit.Courier.Contracts;

namespace Infrastructure.Messaging.Features.Games.Workflows.BuildRemoval.Builders
{
    public class GameBuildRemovalWorkflowRoutingSlipBuilder
    {
        public const string ProcessName = "RemoveGameBuild";

        public RoutingSlipBuilder Build(RemoveGameBuildEvent @event)
        {
            ArgumentNullException.ThrowIfNull(@event);

            var builder = new RoutingSlipBuilder(NewId.NextGuid());
            builder.AddSubscription(
                EndpointHelper.BuildConsumerUri(RoutingSlipCleanUpConsumer.QueueName),
                RoutingSlipEvents.Completed | RoutingSlipEvents.Faulted);

            builder.AddActivity(
                GameBuildRemovalActivityNames.RemoveBuildFilesFromStorage,
                EndpointHelper.BuildExecuteActivityUri(RemoveGameBuildFilesFromStorageActivity.ExecuteEndpointName),
                new RemoveGameBuildFilesFromStorageArguments(
                    @event.GameId,
                    @event.BuildId,
                    @event.BuildStoragePath));

            builder.AddActivity(
                GameBuildRemovalActivityNames.RemoveBuildFromDatabase,
                EndpointHelper.BuildExecuteActivityUri(RemoveGameBuildFromDatabaseActivity.ExecuteEndpointName),
                new RemoveGameBuildFromDatabaseArguments(
                    @event.GameId,
                    @event.BuildId));

            return builder;
        }
    }
}
