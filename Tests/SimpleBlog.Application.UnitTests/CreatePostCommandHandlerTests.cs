using NSubstitute;
using SimpleBlog.Application.Posts.Commands.CreatePost;
using SimpleBlog.Domain.Aggregates.PostAggregate;
using SimpleBlog.Domain.AggregatesModel.PostAggregate;
using SimpleBlog.Domain.Common;
using SimpleBlog.Domain.ValueObjects;

namespace SimpleBlog.Application.UnitTests;

public class CreatePostCommandHandlerTests
{
    private readonly IPostRepository _postRepository;
    private readonly CreatePostCommandHandler _handler;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePostCommandHandlerTests()
    {
        _postRepository = Substitute.For<IPostRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _postRepository.UnitOfWork.Returns(_unitOfWork);
        _handler = new CreatePostCommandHandler(_postRepository);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ReturnsSuccess()
    {
        var command = new CreatePostCommand("Test Title", "Valid content", Guid.NewGuid().ToString());
        var post = Post.Create(command.Title, Content.Create(command.Content).Value, UserId.Create(command.AuthorId).Value).Value;
        _postRepository.Add(Arg.Any<Post>()).Returns(post);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(post.Id, result.Value);
        await _postRepository.UnitOfWork.Received(1).SaveEntitiesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithNullCommand_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _handler.Handle(null!, CancellationToken.None));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Handle_WithInvalidContent_ReturnsFailure(string invalidContent)
    {
        var command = new CreatePostCommand("Test Title", invalidContent, Guid.NewGuid().ToString());

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.NotEmpty(result.Error);
        await _postRepository.UnitOfWork.DidNotReceive().SaveEntitiesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithInvalidAuthorId_ReturnsFailure()
    {
        var command = new CreatePostCommand("Test Title", "Valid content", string.Empty);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.NotEmpty(result.Error);
        await _postRepository.UnitOfWork.DidNotReceive().SaveEntitiesAsync(Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Handle_WithInvalidTitle_ReturnsFailure(string invalidTitle)
    {
        var command = new CreatePostCommand(invalidTitle, "Valid content", Guid.NewGuid().ToString());

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.NotEmpty(result.Error);
        await _postRepository.UnitOfWork.DidNotReceive().SaveEntitiesAsync(Arg.Any<CancellationToken>());
    }
}
