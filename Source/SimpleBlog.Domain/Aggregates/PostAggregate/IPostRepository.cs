using SimpleBlog.Domain.AggregatesModel.PostAggregate;
using SimpleBlog.Domain.Common;

namespace SimpleBlog.Domain.Aggregates.PostAggregate;

public interface IPostRepository : IRepository<Post>
{
    Task<Post?> GetByIdAsync(Guid id);

    Task<IEnumerable<Post>> GetAllAsync();

    Post Add(Post post);

    void Update(Post post);

    void AddComment(Post post, Comment comment);
}
