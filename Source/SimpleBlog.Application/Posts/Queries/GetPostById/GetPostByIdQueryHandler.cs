using MediatR;
using SimpleBlog.Application.Posts.Common;
using SimpleBlog.Domain.Aggregates.PostAggregate;
using SimpleBlog.Domain.Common;

namespace SimpleBlog.Application.Posts.Queries.GetPostById;

public class GetPostByIdQueryHandler(IPostRepository postRepository) : IRequestHandler<GetPostByIdQuery, Result<PostDetailsDto>>
{
    private readonly IPostRepository _postRepository = postRepository;

    public async Task<Result<PostDetailsDto>> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var post = await _postRepository.GetByIdAsync(request.Id);
        if (post == null)
            return Result.Failure<PostDetailsDto>($"Post with ID {request.Id} not found");

        var dto = new PostDetailsDto(
            post.Title,
            post.Content.Text,
            post.Comments.Select(c => new CommentDto(c.Id, c.Content.Text, c.CreatedAt)).ToList()
        );
        return Result.Success(dto);
    }
}
