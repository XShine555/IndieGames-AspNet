using Application.Abstractions.Common;
using Application.Genres.Mappers;
using Application.Games.Mappers;
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

            serviceDescriptors.AddScoped<IGameMapper, GameMapper>();
            serviceDescriptors.AddScoped<IGenreMapper, GenreMapper>();
            serviceDescriptors.AddScoped<IUserMapper, UserMapper>();
            serviceDescriptors.AddMediator();
            return serviceDescriptors;
        }
    }
}