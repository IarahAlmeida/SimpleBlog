using MediatR;
using SimpleBlog.Domain.Aggregates.PostAggregate;
using SimpleBlog.Domain.Common;
using SimpleBlog.Domain.ValueObjects;

namespace SimpleBlog.Application.Posts.Commands.CreateComment;

public class CreateCommentCommandHandler(IPostRepository postRepository) : IRequestHandler<CreateCommentCommand, Result<Guid>>
{
    private readonly IPostRepository _postRepository = postRepository;

    public async Task<Result<Guid>> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var post = await _postRepository.GetByIdAsync(request.PostId);
        if (post == null)
            return Result.Failure<Guid>($"Post with ID {request.PostId} not found");

        var contentResult = Content.Create(request.Content);
        if (contentResult.IsFailure)
            return Result.Failure<Guid>(contentResult.Error);

        var userIdResult = UserId.Create(); // Generate new GUID for the author
        if (userIdResult.IsFailure)
            return Result.Failure<Guid>(userIdResult.Error);

        var result = post.AddComment(contentResult.Value, userIdResult.Value);
        if (result.IsFailure)
            return Result.Failure<Guid>(result.Error);

        _postRepository.AddComment(post, result.Value);

        await _postRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        return Result.Success(result.Value.Id);
    }
}
