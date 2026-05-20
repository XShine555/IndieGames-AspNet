using Application.Abstractions.Persistence;
using Domain.Entities;
using Domain.Games.Enums;
using Domain.JobTracking;
using Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Infrastructure.Persistence
{
    public static class DatabaseDependencyInjection
    {
        public static IServiceCollection AddDatabase(this IServiceCollection serviceDescriptors, IConfiguration configuration)
        {
            serviceDescriptors
                .AddOptionsWithValidateOnStart<DatabaseConfiguration>()
                .Bind(configuration.GetRequiredSection(DatabaseConfiguration.SectionName))
                .ValidateDataAnnotations();

            serviceDescriptors.AddSingleton(serviceProvider =>
                serviceProvider.GetRequiredService<IOptions<DatabaseConfiguration>>().Value);

            serviceDescriptors.AddSingleton(serviceProvider =>
            {
                var config = serviceProvider.GetRequiredService<DatabaseConfiguration>();
                var builder = new NpgsqlDataSourceBuilder(config.ConnectionString);
                builder.MapEnum<GameArtworkType>();
                builder.MapEnum<GameArtworkProcessingStatus>();
                builder.MapEnum<GamePictureProcessingStatus>();
                builder.MapEnum<GameBuildStatus>();
                builder.MapEnum<JobTrackingStatus>();
                builder.MapEnum<JobTrackingType>();
                return builder.Build();
            });

            serviceDescriptors.AddDbContext<Database>();
            serviceDescriptors.AddScoped<IDatabase>(serviceProvider => serviceProvider.GetRequiredService<Database>());
            serviceDescriptors.AddScoped<IJobTrackingStore, ProcessTrackingStore>();
            serviceDescriptors.AddSingleton<SaveChangesInterceptor, UpdateTimeStampInterceptor>();
            return serviceDescriptors;
        }
    }
}