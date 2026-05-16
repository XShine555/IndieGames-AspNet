using Application.Abstractions.Messaging.Games.V1;
using Infrastructure.Messaging.Consumers;
using Infrastructure.Messaging.Features.Games.Activities;
using Infrastructure.Messaging.Features.Games.Workflows.BuildProcessing.Arguments;
using Infrastructure.Messaging.Helpers;
using MassTransit;
using MassTransit.Courier.Contracts;

namespace Infrastructure.Messaging.Features.Games.Workflows.BuildProcessing.Builders
{
    public class GameBuildWorkflowRoutingSlipBuilder
    {
        public const string ProcessName = "ProcessGameBuildFiles";

        public RoutingSlipBuilder Build(ProcessGameBuildFilesEvent @event)
        {
            ArgumentNullException.ThrowIfNull(@event);

            var builder = new RoutingSlipBuilder(NewId.NextGuid());
            builder.AddSubscription(
                EndpointHelper.BuildConsumerUri(RoutingSlipCleanUpConsumer.QueueName),
                RoutingSlipEvents.Completed | RoutingSlipEvents.Faulted);

            builder.AddActivity(
                GameBuildActivityNames.GenerateManifest,
                EndpointHelper.BuildExecuteActivityUri(GenerateGameBuildManifestActivity.ExecuteEndpointName),
                new GenerateGameBuildManifestArguments(
                    @event.GameId,
                    @event.BuildId,
                    @event.BuildStoragePath));

            builder.AddActivity(
                GameBuildActivityNames.SynchronizeBuildFiles,
                EndpointHelper.BuildExecuteActivityUri(SynchronizeGameBuildFilesActivity.ExecuteEndpointName),
                new SynchronizeGameBuildFilesArguments(
                    @event.GameId,
                    @event.BuildId));

            return builder;
        }
    }
}
