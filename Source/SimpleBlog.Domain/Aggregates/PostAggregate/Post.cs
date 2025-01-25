using SimpleBlog.Domain.Common;
using SimpleBlog.Domain.Events;
using SimpleBlog.Domain.ValueObjects;

namespace SimpleBlog.Domain.AggregatesModel.PostAggregate;

public class Post : Entity, IAggregateRoot
{
    public string Title { get; private init; }

    public Content Content { get; private init; }

    public UserId AuthorId { get; private init; }

    private readonly List<Comment> _comments;

    public IReadOnlyCollection<Comment> Comments => _comments.AsReadOnly();

    protected Post(Guid id, DateTimeOffset createdAt, string title, Content content, UserId authorId) : base(id, createdAt)
    {
        Title = title;
        Content = content;
        AuthorId = authorId;
        _comments = [];
    }

    public static Result<Post> Create(string title, Content content, UserId authorId)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Failure<Post>("Title cannot be empty");

        var post = new Post(Guid.NewGuid(), DateTimeOffset.UtcNow, title, content, authorId);

        post.AddDomainEvent(new PostCreatedDomainEvent(post));
        return Result.Success(post);
    }

    public Result<Comment> AddComment(Content content, UserId authorId)
    {
        var commentResult = Comment.Create(content, authorId, Id);
        if (commentResult.IsFailure)
            return Result.Failure<Comment>(commentResult.Error);
        _comments.Add(commentResult.Value);
        AddDomainEvent(new CommentAddedDomainEvent(commentResult.Value));
        return Result.Success(commentResult.Value);
    }
}
