using Amazon.Runtime;
using Application.Contracts.Infrastructure;
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

            busFactoryConfigurator.ReceiveEndpoint(
                EndpointHelper.BuildExecuteActivityEndpointName(GeneratePictureWorkflowPathsActivity.ExecuteEndpointName),
                endpointConfigurator =>
                {
                    endpointConfigurator.ExecuteActivityHost<GeneratePictureWorkflowPathsActivity, GeneratePictureWorkflowPathsArguments>(busRegistrationContext);
                } );

            busFactoryConfigurator.ReceiveEndpoint(
                EndpointHelper.BuildExecuteActivityEndpointName(DownloadFileFromBucketActivity.ExecuteEndpointName),
                endpointConfigurator =>
                {
                    endpointConfigurator.ExecuteActivityHost<DownloadFileFromBucketActivity, DownloadFileFromBucketArguments>(busRegistrationContext);
                } );

            busFactoryConfigurator.ReceiveEndpoint(
                EndpointHelper.BuildCompensateActivityEndpointName(DownloadFileFromBucketActivity.ExecuteEndpointName),
                endpointConfigurator =>
                {
                    endpointConfigurator.CompensateActivityHost<DownloadFileFromBucketActivity, DownloadFileFromBucketLog>(busRegistrationContext);
                } );

            busFactoryConfigurator.ReceiveEndpoint(
                EndpointHelper.BuildExecuteActivityEndpointName(ResizePictureActivity.ExecuteEndpointName),
                endpointConfigurator =>
                {
                    endpointConfigurator.ExecuteActivityHost<ResizePictureActivity, ResizePictureLocalArguments>(busRegistrationContext);
                } );

            busFactoryConfigurator.ReceiveEndpoint(
                EndpointHelper.BuildCompensateActivityEndpointName(ResizePictureActivity.ExecuteEndpointName),
                endpointConfigurator =>
                {
                    endpointConfigurator.CompensateActivityHost<ResizePictureActivity, ResizePictureLog>(busRegistrationContext);
                } );

            busFactoryConfigurator.ReceiveEndpoint(
                EndpointHelper.BuildExecuteActivityEndpointName(UploadFileToBucketActivity.ExecuteEndpointName),
                endpointConfigurator =>
                {
                    endpointConfigurator.ExecuteActivityHost<UploadFileToBucketActivity, UploadFileToBucketArguments>(busRegistrationContext);
                } );

            busFactoryConfigurator.ReceiveEndpoint(
                EndpointHelper.BuildCompensateActivityEndpointName(UploadFileToBucketActivity.ExecuteEndpointName),
                endpointConfigurator =>
                {
                    endpointConfigurator.CompensateActivityHost<UploadFileToBucketActivity, UploadFileToBucketLog>(busRegistrationContext);
                } );

            busFactoryConfigurator.ReceiveEndpoint(
                EndpointHelper.BuildExecuteActivityEndpointName(SynchronizeGamePicturesActivity.ExecuteEndpointName),
                endpointConfigurator =>
                {
                    endpointConfigurator.ExecuteActivityHost<SynchronizeGamePicturesActivity, SynchronizeGamePicturesArguments>(busRegistrationContext);
                } );
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
    }
}