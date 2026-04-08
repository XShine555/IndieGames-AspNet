using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.S3;
using Application.Contracts.Infrastructure;
using Infrastructure.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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

        public static IServiceCollection AddCognitoService(this IServiceCollection serviceDescriptors, IConfiguration configuration)
        {
            serviceDescriptors
              .AddOptionsWithValidateOnStart<CognitoConfiguration>()
              .Bind(configuration.GetRequiredSection(CognitoConfiguration.SectionName))
              .ValidateDataAnnotations();

            serviceDescriptors.AddSingleton(serviceProvider =>
                serviceProvider.GetRequiredService<IOptions<CognitoConfiguration>>().Value);

            serviceDescriptors.AddScoped<IAmazonCognitoIdentityProvider>(serviceProvider =>
            {
                var cognitoConfig = serviceProvider.GetRequiredService<CognitoConfiguration>();
                return new AmazonCognitoIdentityProviderClient(
                    cognitoConfig.AccessKey,
                    cognitoConfig.SecretKey,
                    cognitoConfig.SessionToken,
                    RegionEndpoint.GetBySystemName(cognitoConfig.Region)
                );
            } );
            return serviceDescriptors;
        }
    }
}