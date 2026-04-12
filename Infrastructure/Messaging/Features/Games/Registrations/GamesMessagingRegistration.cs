using Infrastructure.Messaging.Features.Games.Activities;
using Infrastructure.Messaging.Features.Games.Workflows.PictureProcessing.Arguments;
using MassTransit;

namespace Infrastructure.Messaging.Features.Games.Registrations
{
    internal static class GamesMessagingRegistration
    {
        internal static void AddGamesMessaging(this IBusRegistrationConfigurator options)
        {
            options.AddExecuteActivity<SynchronizeGamePicturesActivity, SynchronizeGamePicturesArguments>();
        }
    }
}
