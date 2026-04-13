using Application.Abstractions.Messaging.Games.V1;
using Infrastructure.Messaging.Features.Games.Workflows.ArtworkProcessing.Builders;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging.Features.Games.Consumers
{
    public class GenerateGameArtworksConsumer(
        GameArtworkWorkflowRoutingSlipBuilder gameArtworkWorkflowRoutingSlipBuilder,
        ILogger<GenerateGameArtworksConsumer> logger)
        : IConsumer<GenerateGameArtworksEvent>
    {
        public const string EndpointName = "generate-game-artworks";

        public async Task Consume(ConsumeContext<GenerateGameArtworksEvent> context)
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
