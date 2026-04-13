using Application.Abstractions.Persistence;
using Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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

            serviceDescriptors.AddDbContext<Database>();
            serviceDescriptors.AddScoped<IDatabase>(serviceProvider => serviceProvider.GetRequiredService<Database>());
            serviceDescriptors.AddScoped<IProcessTrackingStore, ProcessTrackingStore>();
            serviceDescriptors.AddSingleton<SaveChangesInterceptor, UpdateTimeStampInterceptor>();
            return serviceDescriptors;
        }
    }
}