using NSubstitute;
using SimpleBlog.Application.Posts.Queries.GetPosts;
using SimpleBlog.Domain.Aggregates.PostAggregate;
using SimpleBlog.Domain.AggregatesModel.PostAggregate;
using SimpleBlog.Domain.ValueObjects;

namespace SimpleBlog.Application.UnitTests;

public class GetPostsQueryHandlerTests
{
    private readonly IPostRepository _postRepository;
    private readonly GetPostsQueryHandler _handler;

    public GetPostsQueryHandlerTests()
    {
        _postRepository = Substitute.For<IPostRepository>();
        _handler = new GetPostsQueryHandler(_postRepository);
    }

    [Fact]
    public async Task Handle_WithExistingPosts_ReturnsSuccessWithMappedDtos()
    {
        var posts = new List<Post> { CreateTestPost(), CreateTestPost() };
        _postRepository.GetAllAsync().Returns(posts);
        var query = new GetPostsQuery();

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(posts.Count, result.Value.Count());
        Assert.All(result.Value, dto =>
        {
            var post = posts.First(p => p.Id == dto.Id);
            Assert.Equal(post.Title, dto.Title);
            Assert.Equal(post.Comments.Count, dto.CommentCount);
        });
    }

    [Fact]
    public async Task Handle_WithNullQuery_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _handler.Handle(null!, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithEmptyRepository_ReturnsEmptyCollection()
    {
        _postRepository.GetAllAsync().Returns(new List<Post>());

        var result = await _handler.Handle(new GetPostsQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
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
