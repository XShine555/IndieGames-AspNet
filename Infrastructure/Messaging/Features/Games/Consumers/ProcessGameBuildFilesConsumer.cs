using Application.Abstractions.Messaging.Games.V1;
using Application.Abstractions.Persistence;
using Domain.Games.Enums;
using Infrastructure.Messaging.Features.Common.Workflows;
using Infrastructure.Messaging.Features.Games.Workflows.BuildProcessing.Builders;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging.Features.Games.Consumers
{
    public class ProcessGameBuildFilesConsumer(
        IJobTrackingStore processTrackingStore,
        IDatabase database,
        GameBuildWorkflowRoutingSlipBuilder gameBuildWorkflowRoutingSlipBuilder,
        ILogger<ProcessGameBuildFilesConsumer> logger)
        : IConsumer<ProcessGameBuildFilesEvent>
    {
        public const string EndpointName = "process-game-build-files";

        public async Task Consume(ConsumeContext<ProcessGameBuildFilesEvent> context)
        {
            var gameBuild = await database.GameBuilds
                .SingleOrDefaultAsync(gb => gb.Id == context.Message.BuildId && gb.GameId == context.Message.GameId,
                    context.CancellationToken);

            if (gameBuild is null)
            {
                logger.LogWarning("Cannot process build workflow because build {BuildId} was not found", context.Message.BuildId);
                return;
            }

            gameBuild.Status = GameBuildStatus.Processing;
            await database.SaveChangesAsync(context.CancellationToken);

            try
            {
                var processExecutionId = await processTrackingStore.GetOrCreateProcessAsync(
                    GameBuildWorkflowRoutingSlipBuilder.ProcessName,
                    context.CorrelationId,
                    context.ConversationId,
                    context.MessageId,
                    context.CancellationToken);

                var routingSlipBuilder = gameBuildWorkflowRoutingSlipBuilder.Build(context.Message);
                routingSlipBuilder.AddVariable(
                    ProcessTrackingRoutingSlipVariableNames.Workflow.ProcessExecutionId,
                    processExecutionId.ToString("D"));

                var routingSlip = routingSlipBuilder.Build();

                await context.Execute(routingSlip, context.CancellationToken);

                logger.LogDebug("Game build workflow routing slip executed for build id {BuildId}", context.Message.BuildId);
            }
            catch (Exception exception)
            {
                gameBuild.Status = GameBuildStatus.Failed;
                await database.SaveChangesAsync(context.CancellationToken);

                logger.LogError(
                    exception,
                    "Failed to execute game build workflow routing slip for build id {BuildId}",
                    context.Message.BuildId);
                throw;
            }
        }
    }
}
