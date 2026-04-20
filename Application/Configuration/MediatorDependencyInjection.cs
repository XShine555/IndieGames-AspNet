using Application.Abstractions.Common;
using Application.Genres.Mappers;
using Application.Games.Builds.Mappers;
using Application.Games.Catalog.Mappers;
using Application.Games.Media.Mappers;
using Application.Users.Mappers;
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

            serviceDescriptors.AddScoped<IGameCatalogMapper, GameCatalogMapper>();
            serviceDescriptors.AddScoped<IGameMediaMapper, GameMediaMapper>();
            serviceDescriptors.AddScoped<IGameBuildMapper, GameBuildMapper>();
            serviceDescriptors.AddScoped<IGenreMapper, GenreMapper>();
            serviceDescriptors.AddScoped<IUserMapper, UserMapper>();
            serviceDescriptors.AddMediator();
            return serviceDescriptors;
        }
    }
}