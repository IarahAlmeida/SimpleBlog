using MediatR;
using SimpleBlog.Application.Posts.Common;
using SimpleBlog.Domain.Common;

namespace SimpleBlog.Application.Posts.Queries.GetPosts;
public record GetPostsQuery : IRequest<Result<IEnumerable<PostSummaryDto>>>;
