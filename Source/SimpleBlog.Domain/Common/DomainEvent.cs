using MediatR;

namespace SimpleBlog.Domain.Common;

public record DomainEvent : INotification
{
    DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
}
