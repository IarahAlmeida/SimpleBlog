namespace SimpleBlog.Application.Posts.Common;
public record CommentDto(Guid Id, string Content, DateTimeOffset CreatedAt);
