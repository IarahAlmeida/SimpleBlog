using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SimpleBlog.Domain.AggregatesModel.PostAggregate;
using SimpleBlog.Domain.Common;

namespace SimpleBlog.Infrastructure.Persistence.Contexts;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration, IPublisher publisher) : DbContext(options), IUnitOfWork
{
    private readonly IConfiguration _configuration = configuration;
    private readonly IPublisher _publisher = publisher;
    public DbSet<Post> Posts { get; set; }
    public DbSet<Comment> Comments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.Ignore<DomainEvent>();

        base.OnModelCreating(modelBuilder);

        var schema = _configuration.GetRequiredSection("DbSchema").Value;

        modelBuilder.HasDefaultSchema(schema ?? "public");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var domainEvents = ChangeTracker.Entries<Entity>()
            .Select(x => x.Entity)
            .Where(x => x.DomainEvents.Count != 0)
            .SelectMany(x => x.DomainEvents);

        var result = await base.SaveChangesAsync(cancellationToken);

        // NOTE: this could later be changed to the outbox pattern
        foreach (var domainEvent in domainEvents)
        {
            await _publisher.Publish(domainEvent, cancellationToken);
        }

        return result;
    }

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        var domainEvents = ChangeTracker.Entries<Entity>()
            .Select(x => x.Entity)
            .Where(x => x.DomainEvents.Count != 0)
            .SelectMany(x => x.DomainEvents);

        // After executing this line all the changes (from the Command Handler and Domain Event Handlers)
        // performed through the DbContext will be committed
        _ = await base.SaveChangesAsync(cancellationToken);

        // NOTE: this could later be changed to the outbox pattern
        foreach (var domainEvent in domainEvents)
        {
            await _publisher.Publish(domainEvent, cancellationToken);
        }

        return true;
    }
}
