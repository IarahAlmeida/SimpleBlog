using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using SimpleBlog.Application.Posts.Commands.CreatePost;
using SimpleBlog.Domain.Common;

namespace SimpleBlog.WebAPI.UnitTests.PostsEndpoint;

public class CreatePostTests
{
    private readonly IMediator _mediator;
    private readonly CancellationToken _cancellationToken;

    public CreatePostTests()
    {
        _mediator = Substitute.For<IMediator>();
        _cancellationToken = CancellationToken.None;
    }

    [Fact]
    public async Task CreatePost_Success_ReturnsCreatedResult()
    {
        // Arrange
        var command = new CreatePostCommand("Test Title", "Test Content", Guid.NewGuid().ToString());
        var postId = Guid.NewGuid();
        _mediator.Send(command, _cancellationToken)
            .Returns(Result<Guid>.Success(postId));

        // Act
        var result = await PostsEndpoint.CreatePost(command, _mediator, _cancellationToken);

        // Assert
        var createdResult = Assert.IsType<Created<Guid>>(result.Result);
        Assert.Equal(postId, createdResult.Value);
        Assert.Equal($"/api/posts/{postId}", createdResult.Location);
    }

    [Fact]
    public async Task CreatePost_Failure_ReturnsBadRequest()
    {
        // Arrange
        var command = new CreatePostCommand("Test Title", "Test Content", Guid.NewGuid().ToString());
        var errorMessage = "Failed to create post";
        _mediator.Send(command, _cancellationToken)
            .Returns(Result<Guid>.Failure(errorMessage));

        // Act
        var result = await PostsEndpoint.CreatePost(command, _mediator, _cancellationToken);

        // Assert
        var badRequestResult = Assert.IsType<BadRequest<string>>(result.Result);
        Assert.Equal(errorMessage, badRequestResult.Value);
    }
}
