using SimpleBlog.Domain.AggregatesModel.PostAggregate;
using SimpleBlog.Domain.Common;

namespace SimpleBlog.Domain.Events;

public sealed record PostCreatedDomainEvent(Post Post) : DomainEvent;
