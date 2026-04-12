using Amazon.Runtime;
using Application.Abstractions;
using Infrastructure.Messaging.EventBus;
using Infrastructure.Messaging.Features.Common.Activities.Files;
using Infrastructure.Messaging.Features.Common.Activities.Pictures;
using Infrastructure.Messaging.Features.Common.Registrations;
using Infrastructure.Messaging.Features.Games.Activities;
using Infrastructure.Messaging.Features.Games.Consumers;
using Infrastructure.Messaging.Features.Games.Registrations;
using Infrastructure.Messaging.Features.Games.Workflows.PictureProcessing.Arguments;
using Infrastructure.Messaging.Features.Games.Workflows.PictureProcessing.Builders;
using Infrastructure.Messaging.Features.Games.Workflows.PictureProcessing.Logs;
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
    public static class MassTransitDependencyInjection
    {
        public static IServiceCollection AddMassTransitClient(this IServiceCollection serviceDescriptors, IConfiguration configuration)
        {
            serviceDescriptors
                .AddOptionsWithValidateOnStart<MassTransitConfiguration>()
                .Bind(configuration.GetRequiredSection(MassTransitConfiguration.SectionName))
                .ValidateDataAnnotations();

            serviceDescriptors.AddSingleton(serviceProvider =>
                serviceProvider.GetRequiredService<IOptions<MassTransitConfiguration>>().Value);

            serviceDescriptors.AddScoped<IEventBus, MassTransitEventBus>();
            serviceDescriptors.AddMassTransit(options =>
            {
                options.UsingAmazonSqs((busRegistrationContext, busFactoryConfigurator) =>
                    ConfigureAmazonSqsHost(
                        busFactoryConfigurator,
                        busRegistrationContext.GetRequiredService<MassTransitConfiguration>()));
            } );
            return serviceDescriptors;
        }

        public static IServiceCollection AddMassTransitConsumer(this IServiceCollection serviceDescriptors, IConfiguration configuration)
        {
            serviceDescriptors
                .AddOptionsWithValidateOnStart<MassTransitConfiguration>()
                .Bind(configuration.GetRequiredSection(MassTransitConfiguration.SectionName))
                .ValidateDataAnnotations();

            serviceDescriptors.AddSingleton(serviceProvider =>
                serviceProvider.GetRequiredService<IOptions<MassTransitConfiguration>>().Value);

            serviceDescriptors
                .AddOptionsWithValidateOnStart<WorkerConfiguration>()
                .Bind(configuration.GetRequiredSection(WorkerConfiguration.SectionName))
                .ValidateDataAnnotations();

            serviceDescriptors.AddSingleton(serviceProvider =>
                serviceProvider.GetRequiredService<IOptions<WorkerConfiguration>>().Value);

            serviceDescriptors.AddScoped<PictureWorkflowRoutingSlipBuilder>();
            serviceDescriptors.AddScoped<UserProfilePictureWorkflowRoutingSlipBuilder>();
            serviceDescriptors.AddMassTransit(options =>
            {
                options.AddCommonMessaging();
                options.AddGamesMessaging();
                options.AddUsersMessaging();

                options.UsingAmazonSqs((busRegistrationContext, busFactoryConfigurator) =>
                {
                    ConfigureAmazonSqsHost(
                        busFactoryConfigurator,
                        busRegistrationContext.GetRequiredService<MassTransitConfiguration>());

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

            ConfigureExecuteActivityEndpoint<GeneratePictureWorkflowPathsActivity, GeneratePictureWorkflowPathsArguments>(
                busFactoryConfigurator,
                busRegistrationContext,
                GeneratePictureWorkflowPathsActivity.ExecuteEndpointName);

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

            ConfigureExecuteActivityEndpoint<SynchronizeGamePicturesActivity, SynchronizeGamePicturesArguments>(
                busFactoryConfigurator,
                busRegistrationContext,
                SynchronizeGamePicturesActivity.ExecuteEndpointName);

            ConfigureExecuteActivityEndpoint<SynchronizeUserProfilePicturesActivity, SynchronizeUserProfilePicturesArguments>(
                busFactoryConfigurator,
                busRegistrationContext,
                SynchronizeUserProfilePicturesActivity.ExecuteEndpointName);
        }

        private static void ConfigureAmazonSqsHost(
            IAmazonSqsBusFactoryConfigurator busFactoryConfigurator,
            MassTransitConfiguration massTransitConfiguration)
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
