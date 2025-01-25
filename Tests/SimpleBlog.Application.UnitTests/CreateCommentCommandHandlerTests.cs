using NSubstitute;
using SimpleBlog.Application.Posts.Commands.CreateComment;
using SimpleBlog.Domain.Aggregates.PostAggregate;
using SimpleBlog.Domain.AggregatesModel.PostAggregate;
using SimpleBlog.Domain.Common;
using SimpleBlog.Domain.ValueObjects;

namespace SimpleBlog.Application.UnitTests;

public class CreateCommentCommandHandlerTests
{
    private readonly IPostRepository _postRepository;
    private readonly CreateCommentCommandHandler _handler;
    private readonly IUnitOfWork _unitOfWork;
    private readonly Post _post;

    public CreateCommentCommandHandlerTests()
    {
        _postRepository = Substitute.For<IPostRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _postRepository.UnitOfWork.Returns(_unitOfWork);
        _post = Post.Create("Test Post", Content.Create("Test Content").Value, UserId.Create().Value).Value;
        _handler = new CreateCommentCommandHandler(_postRepository);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ReturnsSuccess()
    {
        var command = new CreateCommentCommand(_post.Id, "Valid comment");
        _postRepository.GetByIdAsync(_post.Id).Returns(_post);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value);
        await _unitOfWork.Received(1).SaveEntitiesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithNullCommand_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _handler.Handle(null!, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithNonExistentPost_ReturnsFailure()
    {
        var command = new CreateCommentCommand(Guid.NewGuid(), "Valid comment");
        _postRepository.GetByIdAsync(command.PostId).Returns((Post)null!);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains(command.PostId.ToString(), result.Error, StringComparison.InvariantCultureIgnoreCase);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Handle_WithInvalidContent_ReturnsFailure(string invalidContent)
    {
        var command = new CreateCommentCommand(_post.Id, invalidContent);
        _postRepository.GetByIdAsync(_post.Id).Returns(_post);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.NotEmpty(result.Error);
        await _unitOfWork.DidNotReceive().SaveEntitiesAsync(Arg.Any<CancellationToken>());
    }
}
