using Application.Abstractions.Persistence;
using Application.Payments.Constants;
using Application.Payments.Requests;
using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Payments.Handlers
{
    public class ProcessStripeEventRequestHandler(
        IDatabase database,
        IMediator mediator,
        ILogger<ProcessStripeEventRequestHandler> logger)
        : ICommandHandler<ProcessStripeEventRequest, Result>
    {
        public async ValueTask<Result> Handle(ProcessStripeEventRequest request, CancellationToken cancellationToken)
        {
            var eventProcessing = await database.StripeEventProcessings
                .SingleOrDefaultAsync(current => current.EventId == request.EventId, cancellationToken);

            if (eventProcessing is not null && eventProcessing.Status == Domain.Entities.StripeEventProcessingStatus.Completed)
            {
                logger.LogInformation("Stripe event {EventId} already processed", request.EventId);
                return Result.Success();
            }

            if (eventProcessing is null)
            {
                eventProcessing = new Domain.Entities.StripeEventProcessing
                {
                    EventId = request.EventId,
                    EventType = request.EventType
                };

                await database.StripeEventProcessings.AddAsync(eventProcessing, cancellationToken);
            }
            else
            {
                eventProcessing.Attempts++;
                eventProcessing.Status = Domain.Entities.StripeEventProcessingStatus.Processing;
                eventProcessing.LastError = null;
            }

            await database.SaveChangesAsync(cancellationToken);

            try
            {
                if (request.EventType == StripeEventTypeNames.CheckoutSessionCompleted)
                {
                    await mediator.Send(new ProcessOrderRequest(request.OrderId, request.PaymentIntentId), cancellationToken);
                }

                eventProcessing.Status = Domain.Entities.StripeEventProcessingStatus.Completed;
                eventProcessing.ProcessedAt = DateTime.UtcNow;
                await database.SaveChangesAsync(cancellationToken);

                logger.LogInformation("Stripe event {EventId} processed", request.EventId);
                return Result.Success();
            }
            catch (Exception exception)
            {
                eventProcessing.Status = Domain.Entities.StripeEventProcessingStatus.Failed;
                eventProcessing.LastError = exception.Message;
                await database.SaveChangesAsync(cancellationToken);

                logger.LogError(exception, "Failed processing stripe event {EventId}", request.EventId);
                throw;
            }
        }
    }
}
