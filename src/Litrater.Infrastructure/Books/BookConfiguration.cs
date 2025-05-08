using Litrater.Domain.Books;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Litrater.Infrastructure.Books;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.HasKey(b => b.Id);
        
        builder.Property(b => b.Title)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.Property(b => b.Isbn)
            .IsRequired()
            .HasMaxLength(13);
            
        builder.HasMany(b => b.Reviews)
            .WithOne()
            .HasForeignKey(r => r.BookId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
