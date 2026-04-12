using Infrastructure.Messaging.Features.Users.Activities;
using Infrastructure.Messaging.Features.Users.Consumers;
using Infrastructure.Messaging.Features.Users.Workflows.ProfilePictureProcessing.Arguments;
using MassTransit;

namespace Infrastructure.Messaging.Features.Users.Registrations
{
    internal static class UsersMessagingRegistration
    {
        internal static void AddUsersMessaging(this IBusRegistrationConfigurator options)
        {
            options.AddConsumer<GenerateUsersProfilePicturesConsumer>();
            options.AddExecuteActivity<SynchronizeUserProfilePicturesActivity, SynchronizeUserProfilePicturesArguments>();
        }
    }
}