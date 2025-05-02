using ClipVault.Interfaces;
using ClipVault.Models;
using Microsoft.EntityFrameworkCore;


public class AppDbContext : DbContext, IAppDbContext
{
    public DbSet<Snippet> Snippets { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<SnippetTag> SnippetTags { get; set; }
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

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

        modelBuilder.Entity<Tag>().HasData(
            new Tag { Id = 1, Name = "C#" },
            new Tag { Id = 2, Name = "JavaScript" },
            new Tag { Id = 3, Name = "Python" }
);
    }

}