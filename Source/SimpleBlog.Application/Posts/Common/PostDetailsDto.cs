namespace SimpleBlog.Application.Posts.Common;
public record PostDetailsDto(string Title, string Content, IEnumerable<CommentDto> Comments);
