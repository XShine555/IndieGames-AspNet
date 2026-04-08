using Application.Configuration;
using Application.Contracts.Application;
using Application.Games.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Application
{
    public static class IndieGamesApplicationDependencyInjection
    {
        public static IServiceCollection AddApplicationConfiguration(this IServiceCollection serviceDescriptors, IConfiguration configuration)
        {
            serviceDescriptors
                .AddOptionsWithValidateOnStart<GameConfiguration>()
                .Bind(configuration.GetRequiredSection(GameConfiguration.SectionName))
                .ValidateDataAnnotations();
            serviceDescriptors.AddSingleton(serviceProvider =>
                serviceProvider.GetRequiredService<IOptions<GameConfiguration>>().Value);
            serviceDescriptors.AddSingleton<IGamePicturesHelper, GamePicturesHelper>();

            return serviceDescriptors;
        }
    }
}