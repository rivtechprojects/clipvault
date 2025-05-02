using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SnippetTag>()
            .HasKey(st => new { st.SnippetId, st.TagId });

        modelBuilder.Entity<SnippetTag>()
            .HasOne(st => st.Snippet)
            .WithMany(s => s.SnippetTags)
            .HasForeignKey(st => st.SnippetId);

        modelBuilder.Entity<SnippetTag>()
            .HasOne(st => st.Tag)
            .WithMany(t => t.SnippetTags)
            .HasForeignKey(st => st.TagId);
    }

}