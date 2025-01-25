using NSubstitute;
using SimpleBlog.Application.Posts.Queries.GetPostById;
using SimpleBlog.Domain.Aggregates.PostAggregate;
using SimpleBlog.Domain.AggregatesModel.PostAggregate;
using SimpleBlog.Domain.ValueObjects;

namespace SimpleBlog.Application.UnitTests;

public class GetPostByIdQueryHandlerTests
{
    private readonly IPostRepository _postRepository;
    private readonly GetPostByIdQueryHandler _handler;

    public GetPostByIdQueryHandlerTests()
    {
        _postRepository = Substitute.For<IPostRepository>();
        _handler = new GetPostByIdQueryHandler(_postRepository);
    }

    [Fact]
    public async Task Handle_WithExistingPost_ReturnsSuccessWithMappedDto()
    {
        var postId = Guid.NewGuid();
        var post = CreateTestPost();
        var query = new GetPostByIdQuery(postId);
        _postRepository.GetByIdAsync(postId).Returns(post);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(post.Title, result.Value.Title);
        Assert.Equal(post.Content.Text, result.Value.Content);
        Assert.Equal(post.Comments.Count, result.Value.Comments.Count());
    }

    [Fact]
    public async Task Handle_WithNullQuery_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _handler.Handle(null!, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithNonExistentPost_ReturnsFailure()
    {
        var query = new GetPostByIdQuery(Guid.NewGuid());
        _postRepository.GetByIdAsync(query.Id).Returns((Post)null!);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains(query.Id.ToString(), result.Error, StringComparison.InvariantCulture);
    }

    private static Post CreateTestPost()
    {
        var post = Post.Create(
            "Test Title",
            Content.Create("Test Content").Value,
            UserId.Create().Value).Value;

        post.AddComment(
            Content.Create("Test Comment").Value,
            UserId.Create().Value);

        return post;
    }
}
