using Application.Abstractions.Messaging.Games.V1;
using Application.Abstractions.Persistence;
using Domain.Games.Enums;
using Infrastructure.Messaging.Features.Common.Workflows;
using Infrastructure.Messaging.Features.Games.Workflows.BuildRemoval.Builders;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging.Features.Games.Consumers
{
    public class RemoveGameBuildConsumer(
        IJobTrackingStore processTrackingStore,
        IDatabase database,
        GameBuildRemovalWorkflowRoutingSlipBuilder gameBuildRemovalWorkflowRoutingSlipBuilder,
        ILogger<RemoveGameBuildConsumer> logger)
        : IConsumer<RemoveGameBuildEvent>
    {
        public const string EndpointName = "remove-game-build";

        public async Task Consume(ConsumeContext<RemoveGameBuildEvent> context)
        {
            var gameBuild = await database.GameBuilds
                .SingleOrDefaultAsync(
                    gb => gb.Id == context.Message.BuildId && gb.GameId == context.Message.GameId,
                    context.CancellationToken);

            if (gameBuild is null)
            {
                logger.LogWarning("Cannot execute build removal workflow because build {BuildId} was not found", context.Message.BuildId);
                return;
            }

            gameBuild.Status = GameBuildStatus.Removing;
            await database.SaveChangesAsync(context.CancellationToken);

            try
            {
                var processExecutionId = await processTrackingStore.GetOrCreateProcessAsync(
                    GameBuildRemovalWorkflowRoutingSlipBuilder.ProcessName,
                    context.CorrelationId,
                    context.ConversationId,
                    context.MessageId,
                    context.CancellationToken);

                var routingSlipBuilder = gameBuildRemovalWorkflowRoutingSlipBuilder.Build(context.Message);
                routingSlipBuilder.AddVariable(
                    ProcessTrackingRoutingSlipVariableNames.Workflow.ProcessExecutionId,
                    processExecutionId.ToString("D"));

                var routingSlip = routingSlipBuilder.Build();
                await context.Execute(routingSlip, context.CancellationToken);

                logger.LogDebug("Game build removal workflow routing slip executed for build id {BuildId}", context.Message.BuildId);
            }
            catch (Exception exception)
            {
                gameBuild.Status = GameBuildStatus.Failed;
                await database.SaveChangesAsync(context.CancellationToken);

                logger.LogError(
                    exception,
                    "Failed to execute game build removal workflow routing slip for build id {BuildId}",
                    context.Message.BuildId);
                throw;
            }
        }
    }
}
