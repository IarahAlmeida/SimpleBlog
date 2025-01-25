using MediatR;
using SimpleBlog.Domain.Common;

namespace SimpleBlog.Application.Posts.Commands.CreateComment;
public record CreateCommentCommand(Guid PostId, string Content) : IRequest<Result<Guid>>;
