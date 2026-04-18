using Application.Abstractions.Messaging.Payments.V1;
using Application.Payments.Requests;
using Ardalis.Result;
using MassTransit;
using Mediator;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging.Features.Payments.Consumers
{
    public class StripeCheckoutCompletedConsumer(
        IMediator mediator,
        ILogger<StripeCheckoutCompletedConsumer> logger)
        : IConsumer<StripeCheckoutCompletedEvent>
    {
        public const string EndpointName = "stripe-checkout-completed";

        public async Task Consume(ConsumeContext<StripeCheckoutCompletedEvent> context)
        {
            var request = new ProcessStripeEventRequest(
                context.Message.EventId,
                context.Message.EventType,
                context.Message.OrderId,
                context.Message.CheckoutSessionId,
                context.Message.PaymentIntentId);

            var result = await mediator.Send(request, context.CancellationToken);

            if (result.Status != ResultStatus.Ok)
            {
                logger.LogWarning(
                    "Stripe event {EventId} for order {OrderId} was not processed successfully. Status: {Status}",
                    context.Message.EventId,
                    context.Message.OrderId,
                    result.Status);
            }
        }
    }
}
