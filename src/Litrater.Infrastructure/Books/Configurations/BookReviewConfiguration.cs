using Litrater.Domain.Books;
using Litrater.Infrastructure.Common.Configurations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Litrater.Infrastructure.Books.Configurations;

public class BookReviewConfiguration : EntityConfiguration<BookReview>
{
    public override void Configure(EntityTypeBuilder<BookReview> builder)
    {
        base.Configure(builder);

        builder.Property(r => r.Content)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(r => r.Rating)
            .IsRequired();

        builder.Property(r => r.BookId)
            .IsRequired();

        builder.Property(r => r.UserId)
            .IsRequired();

        builder.HasIndex(r => r.UserId);

        builder.HasIndex(r => r.CreatedDate);

        builder.HasIndex(r => new { r.UserId, r.BookId })
            .IsUnique();
    }
}