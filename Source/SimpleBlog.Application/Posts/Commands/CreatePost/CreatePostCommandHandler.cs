using MediatR;
using SimpleBlog.Domain.Aggregates.PostAggregate;
using SimpleBlog.Domain.AggregatesModel.PostAggregate;
using SimpleBlog.Domain.Common;
using SimpleBlog.Domain.ValueObjects;

namespace SimpleBlog.Application.Posts.Commands.CreatePost;

public class CreatePostCommandHandler(IPostRepository postRepository) : IRequestHandler<CreatePostCommand, Result<Guid>>
{
    private readonly IPostRepository _postRepository = postRepository;

    public async Task<Result<Guid>> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var contentResult = Content.Create(request.Content);
        if (contentResult.IsFailure)
            return Result.Failure<Guid>(contentResult.Error);

        var userIdResult = UserId.Create(request.AuthorId);
        if (userIdResult.IsFailure)
            return Result.Failure<Guid>(userIdResult.Error);

        var postResult = Post.Create(request.Title, contentResult.Value, userIdResult.Value);
        if (postResult.IsFailure)
            return Result.Failure<Guid>(postResult.Error);

        var post = _postRepository.Add(postResult.Value);

        await _postRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Result.Success(post.Id);
    }
}
