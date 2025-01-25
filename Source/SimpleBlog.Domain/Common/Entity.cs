using MediatR;

namespace SimpleBlog.Domain.Common;

public abstract class Entity(Guid id, DateTimeOffset createdAt)
{
    public Guid Id { get; private init; } = id;
    public DateTimeOffset CreatedAt { get; private init; } = createdAt;

    private readonly List<INotification> _domainEvents = [];
    public IReadOnlyCollection<INotification> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(INotification domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}
