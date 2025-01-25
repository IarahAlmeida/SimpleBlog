using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleBlog.Domain.AggregatesModel.PostAggregate;
using SimpleBlog.Domain.ValueObjects;

namespace SimpleBlog.Infrastructure.Persistence.Configurations;

public class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("posts");

        builder.Ignore(b => b.DomainEvents);

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasColumnName("id");

        builder.Property(p => p.Title)
            .HasColumnName("title")
            .IsRequired()
            .HasMaxLength(200);

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
    }
}
