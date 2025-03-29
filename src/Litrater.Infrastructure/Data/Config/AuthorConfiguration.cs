using Litrater.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Litrater.Infrastructure.Data.Config;

public class AuthorConfiguration : IEntityTypeConfiguration<Author>
{
    public void Configure(EntityTypeBuilder<Author> builder)
    {
        builder.HasKey(a => a.Id);
        
        builder.Property(a => a.FirstFirstName)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(a => a.LastName)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.HasMany(a => a.Books)
            .WithMany(b => b.Authors)
            .UsingEntity(j => j.ToTable("AuthorBooks"));
    }
} 