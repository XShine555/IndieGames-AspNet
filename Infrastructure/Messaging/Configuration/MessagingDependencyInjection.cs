using Amazon.Runtime;
using Application.Abstractions.Messaging;
using Infrastructure.Messaging.Consumers;
using Infrastructure.Messaging.EventBus;
using Infrastructure.Messaging.Features.Achievements.Activities;
using Infrastructure.Messaging.Features.Achievements.Consumers;
using Infrastructure.Messaging.Features.Achievements.Registrations;
using Infrastructure.Messaging.Features.Achievements.Workflows.AchievementPictureProcessing.Builders;
using Infrastructure.Messaging.Features.Achievements.Workflows.AchievementPictureProcessing.Arguments;
using Infrastructure.Messaging.Features.Achievements.Workflows.AchievementPictureProcessing.Logs;
using Infrastructure.Messaging.Features.Common.Activities.Files;
using Infrastructure.Messaging.Features.Common.Activities.Pictures;
using Infrastructure.Messaging.Features.Common.Registrations;
using Infrastructure.Messaging.Features.Games.Activities;
using Infrastructure.Messaging.Features.Games.Consumers;
using Infrastructure.Messaging.Features.Games.Registrations;
using Infrastructure.Messaging.Features.Games.Workflows.ArtworkProcessing.Arguments;
using Infrastructure.Messaging.Features.Games.Workflows.ArtworkProcessing.Builders;
using Infrastructure.Messaging.Features.Games.Workflows.BuildProcessing.Arguments;
using Infrastructure.Messaging.Features.Games.Workflows.BuildProcessing.Builders;
using Infrastructure.Messaging.Features.Games.Workflows.BuildRemoval.Arguments;
using Infrastructure.Messaging.Features.Games.Workflows.BuildRemoval.Builders;
using Infrastructure.Messaging.Features.Games.Workflows.StorePictureProcessing.Arguments;
using Infrastructure.Messaging.Features.Games.Workflows.StorePictureProcessing.Builders;
using Infrastructure.Messaging.Features.Games.Workflows.StorePictureProcessing.Logs;
using Infrastructure.Messaging.Features.Users.Activities;
using Infrastructure.Messaging.Features.Users.Consumers;
using Infrastructure.Messaging.Features.Users.Registrations;
using Infrastructure.Messaging.Features.Users.Workflows.ProfilePictureProcessing.Arguments;
using Infrastructure.Messaging.Features.Users.Workflows.ProfilePictureProcessing.Builders;
using Infrastructure.Messaging.Helpers;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Infrastructure.Messaging.Configuration
{
    public static class MessagingDependencyInjection
    {
        public static IServiceCollection AddMassTransitClient(this IServiceCollection serviceDescriptors, IConfiguration configuration)
        {
            serviceDescriptors
                .AddOptionsWithValidateOnStart<MessagingConfiguration>()
                .Bind(configuration.GetRequiredSection(MessagingConfiguration.SectionName))
                .ValidateDataAnnotations();

            serviceDescriptors.AddSingleton(serviceProvider =>
                serviceProvider.GetRequiredService<IOptions<MessagingConfiguration>>().Value);

            serviceDescriptors.AddScoped<IEventBus, MassTransitEventBus>();
            serviceDescriptors.AddMassTransit(options =>
            {
                options.UsingAmazonSqs((busRegistrationContext, busFactoryConfigurator) =>
                    ConfigureAmazonSqsHost(
                        busFactoryConfigurator,
                        busRegistrationContext.GetRequiredService<MessagingConfiguration>()));
            } );
            return serviceDescriptors;
        }

        public static IServiceCollection AddMassTransitConsumer(this IServiceCollection serviceDescriptors, IConfiguration configuration)
        {
            serviceDescriptors
                .AddOptionsWithValidateOnStart<MessagingConfiguration>()
                .Bind(configuration.GetRequiredSection(MessagingConfiguration.SectionName))
                .ValidateDataAnnotations();

            serviceDescriptors.AddSingleton(serviceProvider =>
                serviceProvider.GetRequiredService<IOptions<MessagingConfiguration>>().Value);

            serviceDescriptors
                .AddOptionsWithValidateOnStart<WorkerConfiguration>()
                .Bind(configuration.GetRequiredSection(WorkerConfiguration.SectionName))
                .ValidateDataAnnotations();

            serviceDescriptors.AddSingleton(serviceProvider =>
                serviceProvider.GetRequiredService<IOptions<WorkerConfiguration>>().Value);

            serviceDescriptors.AddScoped<GameStorePictureWorkflowRoutingSlipBuilder>();
            serviceDescriptors.AddScoped<GameArtworkWorkflowRoutingSlipBuilder>();
            serviceDescriptors.AddScoped<GameBuildWorkflowRoutingSlipBuilder>();
            serviceDescriptors.AddScoped<GameBuildRemovalWorkflowRoutingSlipBuilder>();
            serviceDescriptors.AddScoped<UserProfilePictureWorkflowRoutingSlipBuilder>();
            serviceDescriptors.AddScoped<AchievementPictureWorkflowRoutingSlipBuilder>();
            serviceDescriptors.AddMassTransit(options =>
            {
                options.AddCommonMessaging();
                options.AddGamesMessaging();
                options.AddUsersMessaging();
                options.AddAchievementsMessaging();

                options.UsingAmazonSqs((busRegistrationContext, busFactoryConfigurator) =>
                {
                    ConfigureAmazonSqsHost(
                        busFactoryConfigurator,
                        busRegistrationContext.GetRequiredService<MessagingConfiguration>());

                    ConfigurePictureWorkflowEndpoints(busRegistrationContext, busFactoryConfigurator);
                } );
            } );
            return serviceDescriptors;
        }

        private static void ConfigurePictureWorkflowEndpoints(
            IBusRegistrationContext busRegistrationContext,
            IAmazonSqsBusFactoryConfigurator busFactoryConfigurator)
        {
            busFactoryConfigurator.ReceiveEndpoint(
                EndpointHelper.BuildConsumerEndpointName(RoutingSlipCleanUpConsumer.QueueName),
                endpointConfigurator =>
                {
                    endpointConfigurator.ConfigureConsumer<RoutingSlipCleanUpConsumer>(busRegistrationContext);
                } );

            busFactoryConfigurator.ReceiveEndpoint(
                EndpointHelper.BuildConsumerEndpointName(GenerateGamesPicturesConsumer.EndpointName),
                endpointConfigurator =>
                {
                    endpointConfigurator.ConfigureConsumer<GenerateGamesPicturesConsumer>(busRegistrationContext);
                } );

            busFactoryConfigurator.ReceiveEndpoint(
                EndpointHelper.BuildConsumerEndpointName(GenerateUsersProfilePicturesConsumer.EndpointName),
                endpointConfigurator =>
                {
                    endpointConfigurator.ConfigureConsumer<GenerateUsersProfilePicturesConsumer>(busRegistrationContext);
                } );

            busFactoryConfigurator.ReceiveEndpoint(
                EndpointHelper.BuildConsumerEndpointName(GenerateAchievementsPicturesConsumer.EndpointName),
                endpointConfigurator =>
                {
                    endpointConfigurator.ConfigureConsumer<GenerateAchievementsPicturesConsumer>(busRegistrationContext);
                } );

            busFactoryConfigurator.ReceiveEndpoint(
                EndpointHelper.BuildConsumerEndpointName(ProcessNewGameArtworkConsumer.EndpointName),
                endpointConfigurator =>
                {
                    endpointConfigurator.ConfigureConsumer<ProcessNewGameArtworkConsumer>(busRegistrationContext);
                } );

            busFactoryConfigurator.ReceiveEndpoint(
                EndpointHelper.BuildConsumerEndpointName(ProcessGameBuildFilesConsumer.EndpointName),
                endpointConfigurator =>
                {
                    endpointConfigurator.ConfigureConsumer<ProcessGameBuildFilesConsumer>(busRegistrationContext);
                });

            busFactoryConfigurator.ReceiveEndpoint(
                EndpointHelper.BuildConsumerEndpointName(RemoveGameBuildConsumer.EndpointName),
                endpointConfigurator =>
                {
                    endpointConfigurator.ConfigureConsumer<RemoveGameBuildConsumer>(busRegistrationContext);
                });

            ConfigureExecuteActivityEndpoint<GeneratePictureWorkflowPathsActivity, GeneratePictureWorkflowPathsArguments>(
                busFactoryConfigurator,
                busRegistrationContext,
                GeneratePictureWorkflowPathsActivity.ExecuteEndpointName);

            ConfigureExecuteActivityEndpoint<GenerateGameStorePictureWorkflowPathsActivity, GenerateGameStorePictureWorkflowPathsArguments>(
                busFactoryConfigurator,
                busRegistrationContext,
                GenerateGameStorePictureWorkflowPathsActivity.ExecuteEndpointName);

            ConfigureExecuteActivityEndpoint<GenerateGameArtworkWorkflowPathsActivity, GenerateGameArtworkWorkflowPathsArguments>(
                busFactoryConfigurator,
                busRegistrationContext,
                GenerateGameArtworkWorkflowPathsActivity.ExecuteEndpointName);

            ConfigureExecuteActivityEndpoint<GenerateGameBuildManifestActivity, GenerateGameBuildManifestArguments>(
                busFactoryConfigurator,
                busRegistrationContext,
                GenerateGameBuildManifestActivity.ExecuteEndpointName);

            ConfigureExecuteActivityEndpoint<GenerateUserProfilePictureWorkflowPathsActivity, GeneratePictureWorkflowPathsArguments>(
                busFactoryConfigurator,
                busRegistrationContext,
                GenerateUserProfilePictureWorkflowPathsActivity.ExecuteEndpointName);

            ConfigureActivityEndpoint<ProcessAchievementPictureActivity, ProcessAchievementPictureArguments, ProcessAchievementPictureLog>(
                busFactoryConfigurator,
                busRegistrationContext,
                ProcessAchievementPictureActivity.ExecuteEndpointName);

            ConfigureActivityEndpoint<DownloadFileFromBucketActivity, DownloadFileFromBucketArguments, DownloadFileFromBucketLog>(
                busFactoryConfigurator,
                busRegistrationContext,
                DownloadFileFromBucketActivity.ExecuteEndpointName);

            ConfigureActivityEndpoint<ResizePictureActivity, ResizePictureLocalArguments, ResizePictureLog>(
                busFactoryConfigurator,
                busRegistrationContext,
                ResizePictureActivity.ExecuteEndpointName);

            ConfigureActivityEndpoint<UploadFileToBucketActivity, UploadFileToBucketArguments, UploadFileToBucketLog>(
                busFactoryConfigurator,
                busRegistrationContext,
                UploadFileToBucketActivity.ExecuteEndpointName);

            ConfigureExecuteActivityEndpoint<SynchronizeGameStorePicturesActivity, SynchronizeGameStorePicturesArguments>(
                busFactoryConfigurator,
                busRegistrationContext,
                SynchronizeGameStorePicturesActivity.ExecuteEndpointName);

            ConfigureExecuteActivityEndpoint<SynchronizeGameArtworkActivity, SynchronizeGameArtworkArguments>(
                busFactoryConfigurator,
                busRegistrationContext,
                SynchronizeGameArtworkActivity.ExecuteEndpointName);

            ConfigureExecuteActivityEndpoint<SynchronizeUserProfilePicturesActivity, SynchronizeUserProfilePicturesArguments>(
                busFactoryConfigurator,
                busRegistrationContext,
                SynchronizeUserProfilePicturesActivity.ExecuteEndpointName);

            ConfigureExecuteActivityEndpoint<SynchronizeGameBuildFilesActivity, SynchronizeGameBuildFilesArguments>(
                busFactoryConfigurator,
                busRegistrationContext,
                SynchronizeGameBuildFilesActivity.ExecuteEndpointName);

            ConfigureExecuteActivityEndpoint<RemoveGameBuildFilesFromStorageActivity, RemoveGameBuildFilesFromStorageArguments>(
                busFactoryConfigurator,
                busRegistrationContext,
                RemoveGameBuildFilesFromStorageActivity.ExecuteEndpointName);

            ConfigureExecuteActivityEndpoint<RemoveGameBuildFromDatabaseActivity, RemoveGameBuildFromDatabaseArguments>(
                busFactoryConfigurator,
                busRegistrationContext,
                RemoveGameBuildFromDatabaseActivity.ExecuteEndpointName);

            ConfigureExecuteActivityEndpoint<GenerateAchievementPictureWorkflowPathsActivity, GenerateAchievementPictureWorkflowPathsArguments>(
                busFactoryConfigurator,
                busRegistrationContext,
                GenerateAchievementPictureWorkflowPathsActivity.ExecuteEndpointName);

            ConfigureExecuteActivityEndpoint<SynchronizeAchievementPicturesActivity, SynchronizeAchievementPicturesArguments>(
                busFactoryConfigurator,
                busRegistrationContext,
                SynchronizeAchievementPicturesActivity.ExecuteEndpointName);
        }

        private static void ConfigureAmazonSqsHost(
            IAmazonSqsBusFactoryConfigurator busFactoryConfigurator,
            MessagingConfiguration massTransitConfiguration)
        {
            busFactoryConfigurator.Host(massTransitConfiguration.Address, options =>
            {
                options.Credentials(new SessionAWSCredentials(
                    massTransitConfiguration.AccessKey,
                    massTransitConfiguration.SecretKey,
                    massTransitConfiguration.SessionToken));
            } );
        }

        private static void ConfigureExecuteActivityEndpoint<TActivity, TArguments>(
            IAmazonSqsBusFactoryConfigurator busFactoryConfigurator,
            IBusRegistrationContext busRegistrationContext,
            string endpointName)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            busFactoryConfigurator.ReceiveEndpoint(
                EndpointHelper.BuildExecuteActivityEndpointName(endpointName),
                endpointConfigurator =>
                {
                    endpointConfigurator.ExecuteActivityHost<TActivity, TArguments>(busRegistrationContext);
                } );
        }

        private static void ConfigureActivityEndpoint<TActivity, TArguments, TLog>(
            IAmazonSqsBusFactoryConfigurator busFactoryConfigurator,
            IBusRegistrationContext busRegistrationContext,
            string endpointName)
            where TActivity : class, IActivity<TArguments, TLog>
            where TArguments : class
            where TLog : class
        {
            busFactoryConfigurator.ReceiveEndpoint(
                EndpointHelper.BuildExecuteActivityEndpointName(endpointName),
                endpointConfigurator =>
                {
                    endpointConfigurator.ExecuteActivityHost<TActivity, TArguments>(
                        EndpointHelper.BuildCompensateActivityUri(endpointName),
                        busRegistrationContext);
                } );

            busFactoryConfigurator.ReceiveEndpoint(
                EndpointHelper.BuildCompensateActivityEndpointName(endpointName),
                endpointConfigurator =>
                {
                    endpointConfigurator.CompensateActivityHost<TActivity, TLog>(busRegistrationContext);
                } );
        }
    }
}
