using Amazon;
using Amazon.S3;
using Application.Contracts.Application;
using Application.Contracts.Infrastructure;
using Application.Games.Helpers;
using Infrastructure.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services
{
    public static class ServiceDependencyInjection
    {
        public static IServiceCollection AddS3Service(IServiceCollection serviceDescriptors, IConfiguration configuration)
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
            serviceDescriptors.AddScoped<IGamePicturesHelper, GamePicturesHelper>();
            return serviceDescriptors;
        }
    }
}