using Litrater.Domain.Authors;
using Litrater.Domain.Books;
using Litrater.Infrastructure.Authors.Configurations;
using Litrater.Infrastructure.Books.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Litrater.Infrastructure.Data;

public class LitraterDbContext : DbContext
{
    public LitraterDbContext(DbContextOptions<LitraterDbContext> options) : base(options)
    {
    }

    public DbSet<Author> Authors { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<BookReview> BookReviews { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new AuthorConfiguration());
        modelBuilder.ApplyConfiguration(new BookConfiguration());
        modelBuilder.ApplyConfiguration(new BookReviewConfiguration());
    }
}