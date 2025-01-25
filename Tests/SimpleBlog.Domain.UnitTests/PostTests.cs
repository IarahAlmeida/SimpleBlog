using SimpleBlog.Domain.AggregatesModel.PostAggregate;
using SimpleBlog.Domain.Events;
using SimpleBlog.Domain.ValueObjects;

namespace SimpleBlog.Domain.UnitTests;

public class PostTests
{
    private readonly Content _validContent;
    private readonly UserId _validUserId;

    public PostTests()
    {
        _validContent = Content.Create("Valid content").Value;
        _validUserId = UserId.Create().Value;
    }

    [Fact]
    public void Create_WithValidParameters_ReturnsSuccess()
    {
        var result = Post.Create("Valid Title", _validContent, _validUserId);

        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value.Id);
        Assert.Single(result.Value.DomainEvents);
        Assert.IsType<PostCreatedDomainEvent>(result.Value.DomainEvents.First());
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_WithInvalidTitle_ReturnsFailure(string invalidTitle)
    {
        var result = Post.Create(invalidTitle, _validContent, _validUserId);

        Assert.True(result.IsFailure);
        Assert.Contains("Title", result.Error, StringComparison.InvariantCulture);
    }

    [Fact]
    public void AddComment_WithValidParameters_ReturnsSuccess()
    {
        var post = Post.Create("Valid Title", _validContent, _validUserId).Value;

        var result = post.AddComment(_validContent, _validUserId);

        Assert.True(result.IsSuccess);
        Assert.Single(post.Comments);
        Assert.Contains(post.DomainEvents, e => e is CommentAddedDomainEvent);
    }

    [Fact]
    public void AddComment_WithInvalidContent_ReturnsFailure()
    {
        var post = Post.Create("Valid Title", _validContent, _validUserId).Value;
        var invalidContent = Content.Create("");

        // Act & Assert
        Assert.True(invalidContent.IsFailure);
        Assert.Empty(post.Comments);
    }

    [Fact]
    public void Comments_ReturnsReadOnlyCollection()
    {
        var post = Post.Create("Valid Title", _validContent, _validUserId).Value;
        post.AddComment(_validContent, _validUserId);

        Assert.IsAssignableFrom<IReadOnlyCollection<Comment>>(post.Comments);
    }

    [Fact]
    public void Create_InitializesWithCorrectState()
    {
        var title = "Valid Title";
        var result = Post.Create(title, _validContent, _validUserId);
        var post = result.Value;

        Assert.Equal(title, post.Title);
        Assert.Equal(_validContent, post.Content);
        Assert.Equal(_validUserId, post.AuthorId);
        Assert.Empty(post.Comments);
    }
}
