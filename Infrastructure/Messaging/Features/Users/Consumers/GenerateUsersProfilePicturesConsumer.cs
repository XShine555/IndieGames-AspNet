using Application.Abstractions.Messaging.Users.V1;
using Infrastructure.Messaging.Features.Users.Workflows.ProfilePictureProcessing.Builders;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging.Features.Users.Consumers
{
    public class GenerateUsersProfilePicturesConsumer(
        UserProfilePictureWorkflowRoutingSlipBuilder userProfilePictureWorkflowRoutingSlipBuilder,
        ILogger<GenerateUsersProfilePicturesConsumer> logger)
        : IConsumer<GenerateUsersProfilePicturesEvent>
    {
        public const string EndpointName = "generate-users-profile-pictures";

        public async Task Consume(ConsumeContext<GenerateUsersProfilePicturesEvent> context)
        {
            try
            {
                var routingSlip = userProfilePictureWorkflowRoutingSlipBuilder
                    .Build(context.Message)
                    .Build();

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