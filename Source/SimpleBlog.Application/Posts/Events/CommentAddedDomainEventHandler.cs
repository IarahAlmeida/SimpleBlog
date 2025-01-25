using MediatR;
using SimpleBlog.Domain.Events;

namespace SimpleBlog.Application.Posts.Events;

public sealed class CommentAddedDomainEventHandler : INotificationHandler<CommentAddedDomainEvent>
{
    public Task Handle(CommentAddedDomainEvent notification, CancellationToken cancellationToken)
    {
        // NOTE: this could be used to create integration events and send emails to authors about new comments on their posts, for example
        Console.WriteLine(notification);
        return Task.CompletedTask;
    }
}
