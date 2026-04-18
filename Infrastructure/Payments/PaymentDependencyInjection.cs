using Application.Abstractions.Messaging.Payments;
using Application.Abstractions.Payments;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Infrastructure.Payments
{
    public static class PaymentDependencyInjection
    {
        public static IServiceCollection AddStripePayments(this IServiceCollection serviceDescriptors, IConfiguration configuration)
        {
            serviceDescriptors
                .AddOptionsWithValidateOnStart<StripeConfiguration>()
                .Bind(configuration.GetRequiredSection(StripeConfiguration.SectionName))
                .ValidateDataAnnotations();

            serviceDescriptors.AddSingleton(serviceProvider =>
                serviceProvider.GetRequiredService<IOptions<StripeConfiguration>>().Value);

            serviceDescriptors.AddScoped<IPaymentService, StripePaymentService>();
            serviceDescriptors.AddScoped<IStripeEventPublisher, StripeSqsEventPublisher>();
            return serviceDescriptors;
        }
    }
}
