using Infrastructure.Messaging.Features.Achievements.Activities;
using Infrastructure.Messaging.Features.Achievements.Consumers;
using Infrastructure.Messaging.Features.Achievements.Workflows.AchievementPictureProcessing.Arguments;
using Infrastructure.Messaging.Features.Achievements.Workflows.AchievementPictureProcessing.Logs;
using MassTransit;

namespace Infrastructure.Messaging.Features.Achievements.Registrations
{
    internal static class AchievementsMessagingRegistration
    {
        internal static void AddAchievementsMessaging(this IBusRegistrationConfigurator options)
        {
            options.AddConsumer<GenerateAchievementsPicturesConsumer>();
            options.AddActivity<ProcessAchievementPictureActivity, ProcessAchievementPictureArguments, ProcessAchievementPictureLog>();
            options.AddExecuteActivity<GenerateAchievementPictureWorkflowPathsActivity, GenerateAchievementPictureWorkflowPathsArguments>();
            options.AddExecuteActivity<SynchronizeAchievementPicturesActivity, SynchronizeAchievementPicturesArguments>();
        }
    }
}
