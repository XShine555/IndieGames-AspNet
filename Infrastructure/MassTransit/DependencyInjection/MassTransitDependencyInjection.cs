using Amazon.Runtime;
using Application.Abstractions;
using Infrastructure.Configurations;
using Infrastructure.MassTransit.Activities.Files;
using Infrastructure.MassTransit.Activities.Pictures;
using Infrastructure.MassTransit.Arguments;
using Infrastructure.MassTransit.Consumers;
using Infrastructure.MassTransit.EventBus;
using Infrastructure.MassTransit.Helpers;
using Infrastructure.MassTransit.Logs;
using Infrastructure.MassTransit.RoutingSlip.Builders;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Infrastructure.MassTransit.DependencyInjection
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
            serviceDescriptors.AddMassTransit(options =>
            {
                ConfigurePictureWorkflowRegistrations(options);

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

        private static void ConfigurePictureWorkflowRegistrations(IBusRegistrationConfigurator options)
        {
            options.AddConsumer<GenerateGamesPicturesConsumer>();

            options.AddExecuteActivity<GeneratePictureWorkflowPathsActivity, GeneratePictureWorkflowPathsArguments>();

            options.AddActivity<DownloadFileFromBucketActivity, DownloadFileFromBucketArguments, DownloadFileFromBucketLog>();

            options.AddActivity<ResizePictureActivity, ResizePictureLocalArguments, ResizePictureLog>();

            options.AddActivity<UploadFileToBucketActivity, UploadFileToBucketArguments, UploadFileToBucketLog>();

            options.AddExecuteActivity<SynchronizeGamePicturesActivity, SynchronizeGamePicturesArguments>();
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
        }

        static void ConfigureAmazonSqsHost(
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

        static void ConfigureExecuteActivityEndpoint<TActivity, TArguments>(
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

        static void ConfigureActivityEndpoint<TActivity, TArguments, TLog>(
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
