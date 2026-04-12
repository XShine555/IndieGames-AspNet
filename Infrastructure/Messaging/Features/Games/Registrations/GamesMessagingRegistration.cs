using Infrastructure.Messaging.Features.Games.Activities;
using Infrastructure.Messaging.Features.Games.Workflows.StorePictureProcessing.Arguments;
using MassTransit;

namespace Infrastructure.Messaging.Features.Games.Registrations
{
    internal static class GamesMessagingRegistration
    {
        internal static void AddGamesMessaging(this IBusRegistrationConfigurator options)
        {
            options.AddExecuteActivity<SynchronizeGameStorePicturesActivity, SynchronizeGameStorePicturesArguments>();
        }
    }
}
