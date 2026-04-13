using Application.Abstractions.Messaging.Games.V1;
using Infrastructure.Messaging.Features.Games.Workflows.ArtworkProcessing.Builders;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging.Features.Games.Consumers
{
    public class ProcessNewGameArtworkConsumer(
        GameArtworkWorkflowRoutingSlipBuilder gameArtworkWorkflowRoutingSlipBuilder,
        ILogger<ProcessNewGameArtworkConsumer> logger)
        : IConsumer<ProcessNewGameArtworkEvent>
    {
        public const string EndpointName = "process-new-game-artwork";

        public async Task Consume(ConsumeContext<ProcessNewGameArtworkEvent> context)
        {
            try
            {
                var routingSlip = gameArtworkWorkflowRoutingSlipBuilder
                    .Build(context.Message)
                    .Build();

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
