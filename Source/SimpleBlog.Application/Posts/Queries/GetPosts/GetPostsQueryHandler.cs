using MediatR;
using SimpleBlog.Application.Posts.Common;
using SimpleBlog.Domain.Aggregates.PostAggregate;
using SimpleBlog.Domain.Common;

namespace SimpleBlog.Application.Posts.Queries.GetPosts;

public class GetPostsQueryHandler(IPostRepository postRepository) : IRequestHandler<GetPostsQuery, Result<IEnumerable<PostSummaryDto>>>
{
    private readonly IPostRepository _postRepository = postRepository;

    public async Task<Result<IEnumerable<PostSummaryDto>>> Handle(GetPostsQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var posts = await _postRepository.GetAllAsync();
        var dtos = posts.Select(p => new PostSummaryDto(p.Id, p.Title, p.Comments.Count));
        return Result.Success(dtos);
    }
}
