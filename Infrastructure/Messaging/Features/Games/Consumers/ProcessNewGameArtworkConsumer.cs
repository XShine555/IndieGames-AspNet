using Application.Abstractions.Messaging.Games.V1;
using Application.Abstractions.Persistence;
using Infrastructure.Messaging.Features.Common.Workflows;
using Infrastructure.Messaging.Features.Games.Workflows.ArtworkProcessing.Builders;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging.Features.Games.Consumers
{
    public class ProcessNewGameArtworkConsumer(
        IJobTrackingStore processTrackingStore,
        GameArtworkWorkflowRoutingSlipBuilder gameArtworkWorkflowRoutingSlipBuilder,
        ILogger<ProcessNewGameArtworkConsumer> logger)
        : IConsumer<ProcessNewGameArtworkEvent>
    {
        public const string EndpointName = "process-new-game-artwork";

        public async Task Consume(ConsumeContext<ProcessNewGameArtworkEvent> context)
        {
            try
            {
                var processExecutionId = await processTrackingStore.GetOrCreateProcessAsync(
                    GameArtworkWorkflowRoutingSlipBuilder.ProcessName,
                    context.CorrelationId,
                    context.ConversationId,
                    context.MessageId,
                    context.CancellationToken);

                var routingSlipBuilder = gameArtworkWorkflowRoutingSlipBuilder.Build(context.Message);
                routingSlipBuilder.AddVariable(ProcessTrackingRoutingSlipVariableNames.Workflow.ProcessExecutionId, processExecutionId.ToString("D"));

                var routingSlip = routingSlipBuilder.Build();

                await context.Execute(routingSlip, context.CancellationToken);

                logger.LogDebug("Game artwork workflow routing slip executed for artwork id {ArtworkId}",
                    context.Message.ArtworkId);
            }
            catch (Exception exception)
            {
                logger.LogError(exception,
                    "Failed to execute game artwork workflow routing slip for artwork id {ArtworkId}",
                    context.Message.ArtworkId);
                throw;
            }
        }
    }
}
