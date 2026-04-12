using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Application.Configuration
{
    public static class MediatorDependencyInjection
    {
        public static IServiceCollection AddApplicationMediator(this IServiceCollection serviceDescriptors, IConfiguration configuration)
        {
            serviceDescriptors
                .AddOptionsWithValidateOnStart<GameConfiguration>()
                .Bind(configuration.GetRequiredSection(GameConfiguration.SectionName))
                .ValidateDataAnnotations();
            serviceDescriptors.AddSingleton(serviceProvider =>
                serviceProvider.GetRequiredService<IOptions<GameConfiguration>>().Value);

            serviceDescriptors
                .AddOptionsWithValidateOnStart<UserConfiguration>()
                .Bind(configuration.GetRequiredSection(UserConfiguration.SectionName))
                .ValidateDataAnnotations();
            serviceDescriptors.AddSingleton(serviceProvider =>
                serviceProvider.GetRequiredService<IOptions<UserConfiguration>>().Value);

            serviceDescriptors.AddMediator();
            return serviceDescriptors;
        }
    }
}