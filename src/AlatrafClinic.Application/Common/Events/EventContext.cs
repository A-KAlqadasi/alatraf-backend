namespace AlatrafClinic.Application.Common.Events;

public interface IEventContext
{
    Guid? CurrentMessageId { get; set; }
}

public interface IIntegrationEvent
{
}
