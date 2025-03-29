using Litrater.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Litrater.Infrastructure.Data.Config;

public class BookReviewConfiguration : IEntityTypeConfiguration<BookReview>
{
    public void Configure(EntityTypeBuilder<BookReview> builder)
    {
        builder.HasKey(r => r.Id);
        
        builder.Property(r => r.Content)
            .IsRequired()
            .HasMaxLength(1000);
            
        builder.Property(r => r.Rating)
            .IsRequired();
            
        builder.Property(r => r.BookId)
            .IsRequired();
    }
} 