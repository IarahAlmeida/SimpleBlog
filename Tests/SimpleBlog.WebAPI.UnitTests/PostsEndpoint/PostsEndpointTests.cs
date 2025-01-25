using MediatR;
using NSubstitute;

namespace SimpleBlog.WebAPI.UnitTests.PostsEndpoint;

public class PostsEndpointTests
{
    private readonly IMediator _mediator;
    private readonly CancellationToken _cancellationToken;

    public PostsEndpointTests()
    {
        _mediator = Substitute.For<IMediator>();
        _cancellationToken = CancellationToken.None;
    }

    public class GetPostsTests : PostsEndpointTests
    {
        [Fact]
        public async Task GetPosts_Success_ReturnsOkResult()
        {
            // Arrange
            var posts = new List<PostSummaryDto>
            {
                new("Test Title", DateTime.UtcNow, "Test Author")
            };
            _mediator.Send(Arg.Any<GetPostsQuery>(), _cancellationToken)
                .Returns(Result<IEnumerable<PostSummaryDto>>.Success(posts));

            // Act
            var result = await PostsEndpoint.GetPosts(_mediator, _cancellationToken);

            // Assert
            var okResult = Assert.IsType<Ok<IEnumerable<PostSummaryDto>>>(result.Result);
            Assert.Equal(posts, okResult.Value);
        }

        [Fact]
        public async Task GetPosts_Failure_ReturnsBadRequest()
        {
            // Arrange
            var errorMessage = "Failed to get posts";
            _mediator.Send(Arg.Any<GetPostsQuery>(), _cancellationToken)
                .Returns(Result<IEnumerable<PostSummaryDto>>.Failure(errorMessage));

            // Act
            var result = await PostsEndpoint.GetPosts(_mediator, _cancellationToken);

            // Assert
            var badRequestResult = Assert.IsType<BadRequest<string>>(result.Result);
            Assert.Equal(errorMessage, badRequestResult.Value);
        }
    }

    public class GetPostByIdTests : PostsEndpointTests
    {
        [Fact]
        public async Task GetPostById_Success_ReturnsOkResult()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var postDetails = new PostDetailsDto(
                "Test Title",
                "Test Content",
                DateTime.UtcNow,
                "Test Author",
                new List<CommentDto>());
            _mediator.Send(Arg.Is<GetPostByIdQuery>(q => q.Id == postId), _cancellationToken)
                .Returns(Result<PostDetailsDto>.Success(postDetails));

            // Act
            var result = await PostsEndpoint.GetPostById(postId, _mediator, _cancellationToken);

            // Assert
            var okResult = Assert.IsType<Ok<PostDetailsDto>>(result.Result);
            Assert.Equal(postDetails, okResult.Value);
        }

        [Fact]
        public async Task GetPostById_NotFound_ReturnsNotFound()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var errorMessage = "Post not found";
            _mediator.Send(Arg.Is<GetPostByIdQuery>(q => q.Id == postId), _cancellationToken)
                .Returns(Result<PostDetailsDto>.Failure(errorMessage));

            // Act
            var result = await PostsEndpoint.GetPostById(postId, _mediator, _cancellationToken);

            // Assert
            var notFoundResult = Assert.IsType<NotFound<string>>(result.Result);
            Assert.Equal(errorMessage, notFoundResult.Value);
        }
    }

    public class CreatePostCommentTests : PostsEndpointTests
    {
        [Fact]
        public async Task CreatePostComment_Success_ReturnsCreatedResult()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var commentId = Guid.NewGuid();
            var request = new AddCommentDto("Test Comment");
            _mediator.Send(
                Arg.Is<CreateCommentCommand>(c => c.PostId == postId && c.Content == request.Content),
                _cancellationToken)
                .Returns(Result<Guid>.Success(commentId));

            // Act
            var result = await PostsEndpoint.CreatePostComment(postId, request, _mediator, _cancellationToken);

            // Assert
            var createdResult = Assert.IsType<Created<Guid>>(result.Result);
            Assert.Equal(commentId, createdResult.Value);
            Assert.Equal($"/api/posts/{postId}/comments/{commentId}", createdResult.Location);
        }

        [Fact]
        public async Task CreatePostComment_Failure_ReturnsBadRequest()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var request = new AddCommentDto("Test Comment");
            var errorMessage = "Failed to create comment";
            _mediator.Send(
                Arg.Is<CreateCommentCommand>(c => c.PostId == postId && c.Content == request.Content),
                _cancellationToken)
                .Returns(Result<Guid>.Failure(errorMessage));

            // Act
            var result = await PostsEndpoint.CreatePostComment(postId, request, _mediator, _cancellationToken);

            // Assert
            var badRequestResult = Assert.IsType<BadRequest<string>>(result.Result);
            Assert.Equal(errorMessage, badRequestResult.Value);
        }
    }
}
