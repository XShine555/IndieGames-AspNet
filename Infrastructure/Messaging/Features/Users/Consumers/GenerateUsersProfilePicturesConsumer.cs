using Application.Abstractions.Messaging.Users.V1;
using Application.Abstractions.Persistence;
using Infrastructure.Messaging.Features.Common.Workflows;
using Infrastructure.Messaging.Features.Users.Workflows.ProfilePictureProcessing.Builders;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging.Features.Users.Consumers
{
    public class GenerateUsersProfilePicturesConsumer(
        IProcessTrackingStore processTrackingStore,
        UserProfilePictureWorkflowRoutingSlipBuilder userProfilePictureWorkflowRoutingSlipBuilder,
        ILogger<GenerateUsersProfilePicturesConsumer> logger)
        : IConsumer<GenerateUsersProfilePicturesEvent>
    {
        public const string EndpointName = "generate-users-profile-pictures";

        public async Task Consume(ConsumeContext<GenerateUsersProfilePicturesEvent> context)
        {
            try
            {
                var processExecutionId = await processTrackingStore.GetOrCreateProcessAsync(
                    UserProfilePictureWorkflowRoutingSlipBuilder.ProcessName,
                    context.CorrelationId,
                    context.ConversationId,
                    context.MessageId,
                    context.CancellationToken);

                var routingSlipBuilder = userProfilePictureWorkflowRoutingSlipBuilder.Build(context.Message);
                routingSlipBuilder.AddVariable(ProcessTrackingRoutingSlipVariableNames.Workflow.ProcessExecutionId, processExecutionId.ToString("D"));

                var routingSlip = routingSlipBuilder.Build();

                await context.Execute(routingSlip, context.CancellationToken);

                logger.LogDebug("User profile picture workflow routing slip executed for picture id {PictureId}",
                    context.Message.PictureId);
            }
            catch (Exception exception)
            {
                logger.LogError(exception,
                    "Failed to execute user profile picture workflow routing slip for picture id {PictureId}",
                    context.Message.PictureId);
                throw;
            }
        }
    }
}