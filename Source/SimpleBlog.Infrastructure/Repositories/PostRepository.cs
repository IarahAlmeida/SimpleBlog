using Microsoft.EntityFrameworkCore;
using SimpleBlog.Domain.Aggregates.PostAggregate;
using SimpleBlog.Domain.AggregatesModel.PostAggregate;
using SimpleBlog.Domain.Common;
using SimpleBlog.Infrastructure.Persistence.Contexts;

namespace SimpleBlog.Infrastructure.Repositories;

public class PostRepository(ApplicationDbContext context) : IPostRepository
{
    private readonly ApplicationDbContext _dbContext = context;
    public IUnitOfWork UnitOfWork => _dbContext;

    public async Task<Post?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Posts
            .Include(p => p.Comments)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Post>> GetAllAsync()
    {
        return await _dbContext.Posts
            .Include(p => p.Comments)
            .ToListAsync();
    }

    public Post Add(Post post)
    {
        return _dbContext.Posts.Add(post).Entity;
    }

    public void Update(Post post)
    {
        _dbContext.Entry(post).State = EntityState.Modified;
    }

    public void AddComment(Post post, Comment comment)
    {
        _dbContext.Entry(comment).State = EntityState.Added;
        Update(post);
    }
}
