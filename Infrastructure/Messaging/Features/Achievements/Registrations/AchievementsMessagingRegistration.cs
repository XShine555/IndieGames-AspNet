using Infrastructure.Messaging.Features.Achievements.Activities;
using Infrastructure.Messaging.Features.Achievements.Consumers;
using MassTransit;

namespace Infrastructure.Messaging.Features.Achievements.Registrations
{
    internal static class AchievementsMessagingRegistration
    {
        internal static void AddAchievementsMessaging(this IBusRegistrationConfigurator options)
        {
            options.AddConsumer<GenerateAchievementsPicturesConsumer>();
            options.AddActivity<ProcessAchievementPictureActivity, ProcessAchievementPictureArguments, ProcessAchievementPictureLog>();
        }
    }
}
