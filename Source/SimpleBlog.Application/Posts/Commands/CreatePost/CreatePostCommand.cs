using MediatR;
using SimpleBlog.Domain.Common;

namespace SimpleBlog.Application.Posts.Commands.CreatePost;
public record CreatePostCommand(string Title, string Content, string AuthorId) : IRequest<Result<Guid>>;
