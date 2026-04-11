using Application.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationMediator(this IServiceCollection serviceDescriptors, IConfiguration configuration)
        {
            serviceDescriptors
                .AddOptionsWithValidateOnStart<GameConfiguration>()
                .Bind(configuration.GetRequiredSection(GameConfiguration.SectionName))
                .ValidateDataAnnotations();
            serviceDescriptors.AddSingleton(serviceProvider =>
                serviceProvider.GetRequiredService<IOptions<GameConfiguration>>().Value);

            serviceDescriptors.AddMediator();
            return serviceDescriptors;
        }
    }
}