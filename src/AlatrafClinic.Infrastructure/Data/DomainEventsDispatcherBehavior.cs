using MediatR;
using AlatrafClinic.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Infrastructure.Data;

public class DomainEventsDispatcherBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IAppDbContext _db;
    private readonly IMediator _mediator;
    private readonly ILogger<DomainEventsDispatcherBehavior<TRequest, TResponse>> _logger;

    public DomainEventsDispatcherBehavior(IAppDbContext db, IMediator mediator, ILogger<DomainEventsDispatcherBehavior<TRequest, TResponse>> logger)
    {
        _db = db;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Execute handler (which should call SaveChanges)
        var response = await next();

        // After handler finishes (and SaveChanges persisted), collect domain events from DbContext
        if (_db is AlatrafClinicDbContext concreteDb)
        {
            var domainEvents = concreteDb.GetDomainEvents();
            if (domainEvents != null && domainEvents.Count > 0)
            {
                _logger.LogInformation("Publishing {Count} domain event(s) after request {RequestType}", domainEvents.Count, typeof(TRequest).FullName);
                foreach (var domainEvent in domainEvents)
                {
                    try
                    {
                        _logger.LogDebug("Publishing domain event {EventType}", domainEvent.GetType().FullName);
                        await _mediator.Publish(domainEvent, cancellationToken);
                        _logger.LogDebug("Published domain event {EventType}", domainEvent.GetType().FullName);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error publishing domain event {EventType}", domainEvent.GetType().FullName);
                        throw;
                    }
                }
            }
        }

        return response;
    }
}
