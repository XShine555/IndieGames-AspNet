using Application.Abstractions.Messaging.Achievements.V1;
using Application.Abstractions.Persistence;
using Infrastructure.Messaging.Features.Common.Workflows;
using Infrastructure.Messaging.Features.Achievements.Workflows.AchievementPictureProcessing.Builders;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging.Features.Achievements.Consumers
{
    public class GenerateAchievementsPicturesConsumer(
        IJobTrackingStore processTrackingStore,
        AchievementPictureWorkflowRoutingSlipBuilder achievementPictureWorkflowRoutingSlipBuilder,
        ILogger<GenerateAchievementsPicturesConsumer> logger)
        : IConsumer<GenerateAchievementsPicturesEvent>
    {
        public const string EndpointName = "generate-achievements-pictures";

        public async Task Consume(ConsumeContext<GenerateAchievementsPicturesEvent> context)
        {
            try
            {
                var processExecutionId = await processTrackingStore.GetOrCreateProcessAsync(
                    AchievementPictureWorkflowRoutingSlipBuilder.ProcessName,
                    context.CorrelationId,
                    context.ConversationId,
                    context.MessageId,
                    context.CancellationToken);

                var routingSlipBuilder = achievementPictureWorkflowRoutingSlipBuilder.Build(context.Message);
                routingSlipBuilder.AddVariable(ProcessTrackingRoutingSlipVariableNames.Workflow.ProcessExecutionId, processExecutionId.ToString("D"));

                var routingSlip = routingSlipBuilder.Build();
                await context.Execute(routingSlip, context.CancellationToken);

                logger.LogDebug("Achievement picture workflow routing slip executed for achievement id {AchievementId}", context.Message.AchievementId);
            }
            catch (Exception exception)
            {
                logger.LogError(exception,
                    "Failed to execute achievement picture workflow routing slip for achievement id {AchievementId}",
                    context.Message.AchievementId);
                throw;
            }
        }
    }
}
