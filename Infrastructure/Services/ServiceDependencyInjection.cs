using Amazon;
using Amazon.S3;
using Application.Abstractions.Payment;
using Application.Abstractions.Storage;
using Infrastructure.Configurations;
using Infrastructure.Services.Stripe;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Stripe;

namespace Infrastructure.Services
{
    public static class ServiceDependencyInjection
    {
        public static IServiceCollection AddS3Service(this IServiceCollection serviceDescriptors, IConfiguration configuration)
        {
            serviceDescriptors
              .AddOptionsWithValidateOnStart<S3Configuration>()
              .Bind(configuration.GetRequiredSection(S3Configuration.SectionName))
              .ValidateDataAnnotations();

            serviceDescriptors.AddSingleton(serviceProvider =>
                serviceProvider.GetRequiredService<IOptions<S3Configuration>>().Value);

            serviceDescriptors.AddScoped<IAmazonS3>(serviceProvider =>
            {
                var s3Config = serviceProvider.GetRequiredService<S3Configuration>();
                return new AmazonS3Client(
                    s3Config.AccessKey,
                    s3Config.SecretKey,
                    s3Config.SessionToken,
                    RegionEndpoint.GetBySystemName(s3Config.Region)
                );
            } );
            serviceDescriptors.AddScoped<IS3Service, S3Service>();
            return serviceDescriptors;
        }

        public static IServiceCollection AddPictureService(this IServiceCollection services)
        {
            services.AddScoped<IPictureService, PictureService>();
            return services;
        }

        public static IServiceCollection AddStripeService(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddOptionsWithValidateOnStart<StripeSettings>()
                .Bind(configuration.GetRequiredSection(StripeSettings.SectionName))
                .ValidateDataAnnotations();

            services.AddSingleton(sp =>
                sp.GetRequiredService<IOptions<StripeSettings>>().Value);

            services.AddSingleton<IStripeClient>(sp =>
                new StripeClient(sp.GetRequiredService<StripeSettings>().SecretKey));

            services.AddScoped<IStripeService, StripeService>();
            return services;
        }
    }
}