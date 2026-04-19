using Infrastructure.Messaging.Features.Games.Activities;
using Infrastructure.Messaging.Features.Games.Consumers;
using Infrastructure.Messaging.Features.Games.Workflows.ArtworkProcessing.Arguments;
using Infrastructure.Messaging.Features.Games.Workflows.BuildProcessing.Arguments;
using Infrastructure.Messaging.Features.Games.Workflows.StorePictureProcessing.Arguments;
using MassTransit;

namespace Infrastructure.Messaging.Features.Games.Registrations
{
    internal static class GamesMessagingRegistration
    {
        internal static void AddGamesMessaging(this IBusRegistrationConfigurator options)
        {
            options.AddConsumer<GenerateGamesPicturesConsumer>();
            options.AddConsumer<ProcessNewGameArtworkConsumer>();
            options.AddConsumer<ProcessGameBuildFilesConsumer>();

            options.AddExecuteActivity<GenerateGameStorePictureWorkflowPathsActivity, GenerateGameStorePictureWorkflowPathsArguments>();
            options.AddExecuteActivity<GenerateGameArtworkWorkflowPathsActivity, GenerateGameArtworkWorkflowPathsArguments>();
            options.AddExecuteActivity<GenerateGameBuildManifestActivity, GenerateGameBuildManifestArguments>();
            options.AddExecuteActivity<SynchronizeGameStorePicturesActivity, SynchronizeGameStorePicturesArguments>();
            options.AddExecuteActivity<SynchronizeGameArtworkActivity, SynchronizeGameArtworkArguments>();
            options.AddExecuteActivity<SynchronizeGameBuildFilesActivity, SynchronizeGameBuildFilesArguments>();
        }
    }
}
