using MediatR;
using SimpleBlog.Domain.Events;

namespace SimpleBlog.Application.Posts.Events;

public sealed class PostCreatedDomainEventHandler : INotificationHandler<PostCreatedDomainEvent>
{
    public Task Handle(PostCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        // NOTE: this could be used to create integration events and send emails to users following authors, for example
        Console.WriteLine(notification);
        return Task.CompletedTask;
    }
}
