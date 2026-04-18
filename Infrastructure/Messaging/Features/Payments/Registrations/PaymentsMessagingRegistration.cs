using Infrastructure.Messaging.Features.Payments.Consumers;
using MassTransit;

namespace Infrastructure.Messaging.Features.Payments.Registrations
{
    internal static class PaymentsMessagingRegistration
    {
        internal static void AddPaymentsMessaging(this IBusRegistrationConfigurator options)
        {
            options.AddConsumer<StripeCheckoutCompletedConsumer>();
        }
    }
}
