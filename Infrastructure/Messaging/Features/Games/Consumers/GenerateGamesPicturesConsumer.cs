using Application.Abstractions.Messaging.Games.V1;
using Application.Abstractions.Persistence;
using Infrastructure.Messaging.Features.Common.Workflows;
using Infrastructure.Messaging.Features.Games.Workflows.StorePictureProcessing.Builders;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging.Features.Games.Consumers
{
    public class GenerateGamesPicturesConsumer(
        IProcessTrackingStore processTrackingStore,
        GameStorePictureWorkflowRoutingSlipBuilder gameStorePictureWorkflowRoutingSlipBuilder,
        ILogger<GenerateGamesPicturesConsumer> logger)
        : IConsumer<GenerateGamesPicturesEvent>
    {
        public const string EndpointName = "generate-games-pictures";

        public async Task Consume(ConsumeContext<GenerateGamesPicturesEvent> context)
        {
            try
            {
                var processExecutionId = await processTrackingStore.GetOrCreateProcessAsync(
                    GameStorePictureWorkflowRoutingSlipBuilder.ProcessName,
                    context.CorrelationId,
                    context.ConversationId,
                    context.MessageId,
                    context.CancellationToken);

                var routingSlipBuilder = gameStorePictureWorkflowRoutingSlipBuilder.Build(context.Message);
                routingSlipBuilder.AddVariable(ProcessTrackingRoutingSlipVariableNames.Workflow.ProcessExecutionId, processExecutionId.ToString("D"));

                var routingSlip = routingSlipBuilder.Build();

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
