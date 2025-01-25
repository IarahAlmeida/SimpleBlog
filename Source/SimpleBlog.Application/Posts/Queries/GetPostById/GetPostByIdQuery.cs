using MediatR;
using SimpleBlog.Application.Posts.Common;
using SimpleBlog.Domain.Common;

namespace SimpleBlog.Application.Posts.Queries.GetPostById;
public record GetPostByIdQuery(Guid Id) : IRequest<Result<PostDetailsDto>>;
