using Application.Abstractions.Messaging.Games.V1;
using Infrastructure.Messaging.Features.Games.Workflows.PictureProcessing.Builders;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging.Features.Games.Consumers
{
    public class GenerateGamesPicturesConsumer(
        PictureWorkflowRoutingSlipBuilder pictureWorkflowRoutingSlipBuilder,
        ILogger<GenerateGamesPicturesConsumer> logger)
        : IConsumer<GenerateGamesPicturesEvent>
    {
        public const string EndpointName = "generate-games-pictures";

        public async Task Consume(ConsumeContext<GenerateGamesPicturesEvent> context)
        {
            try
            {
                var routingSlip = pictureWorkflowRoutingSlipBuilder
                    .Build(context.Message)
                    .Build();

                await context.Execute(routingSlip, context.CancellationToken);

                logger.LogDebug("Picture workflow routing slip executed for picture id {PictureId}",
                    context.Message.PictureId);
            }
            catch (Exception exception)
            {
                logger.LogError(exception,
                    "Failed to execute picture workflow routing slip for picture id {PictureId}",
                    context.Message.PictureId);
                throw;
            }
        }
    }
}
