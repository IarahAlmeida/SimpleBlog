using SimpleBlog.Domain.Common;
using SimpleBlog.Domain.ValueObjects;

namespace SimpleBlog.Domain.AggregatesModel.PostAggregate;

public class Comment : Entity
{
    public Content Content { get; private init; }
    public UserId AuthorId { get; private init; }
    public Guid PostId { get; private init; }

    protected Comment(Guid id, DateTimeOffset createdAt, Content content, UserId authorId, Guid postId) : base(id, createdAt)
    {
        Content = content;
        AuthorId = authorId;
        PostId = postId;
    }

    public static Result<Comment> Create(Content content, UserId author, Guid postId)
    {
        var comment = new Comment(Guid.NewGuid(), DateTimeOffset.UtcNow, content, author, postId);

        return Result.Success(comment);
    }
}
