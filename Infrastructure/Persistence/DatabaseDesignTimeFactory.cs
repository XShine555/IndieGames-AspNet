using Domain.Entities;
using Domain.Games.Enums;
using Domain.JobTracking;
using Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Infrastructure.Persistence
{
    public class DatabaseDesignTimeFactory : IDesignTimeDbContextFactory<Database>
    {
        public Database CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("DesignSettings.json")
                .AddUserSecrets<DatabaseDesignTimeFactory>()
                .Build();

            var databaseConfiguration = configuration
                .GetRequiredSection(DatabaseConfiguration.SectionName)
                .Get<DatabaseConfiguration>()
                ?? throw new InvalidOperationException($"{DatabaseConfiguration.SectionName} configuration section not found.");

            var dataSourceBuilder = new NpgsqlDataSourceBuilder(databaseConfiguration.ConnectionString);
            dataSourceBuilder.MapEnum<GameArtworkType>();
            dataSourceBuilder.MapEnum<GameArtworkProcessingStatus>();
            dataSourceBuilder.MapEnum<GamePictureProcessingStatus>();
            dataSourceBuilder.MapEnum<GameBuildStatus>();
            dataSourceBuilder.MapEnum<JobTrackingStatus>();
            dataSourceBuilder.MapEnum<JobTrackingType>();

            return new Database(new UpdateTimeStampInterceptor(), dataSourceBuilder.Build());
        }
    }
}