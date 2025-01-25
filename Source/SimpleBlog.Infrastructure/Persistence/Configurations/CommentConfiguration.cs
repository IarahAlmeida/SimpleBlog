using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleBlog.Domain.AggregatesModel.PostAggregate;
using SimpleBlog.Domain.ValueObjects;

namespace SimpleBlog.Infrastructure.Persistence.Configurations;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("comments");

        builder.Ignore(b => b.DomainEvents);

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasColumnName("id");

        builder.Property(p => p.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(b => b.Content)
            .HasColumnName("content")
            .HasConversion(content => content.Text, value => new Content(value));

        builder.Property(b => b.AuthorId)
            .HasColumnName("author_id")
            .HasConversion(authorId => authorId.Id, value => new UserId(value));

        builder.Property(c => c.PostId)
            .HasColumnName("post_id");
    }
}
