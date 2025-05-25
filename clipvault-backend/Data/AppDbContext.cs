using ClipVault.Interfaces;
using ClipVault.Models;
using Microsoft.EntityFrameworkCore;


public class AppDbContext : DbContext, IAppDbContext
{
    public DbSet<Snippet> Snippets { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<SnippetTag> SnippetTags { get; set; }
    public DbSet<Language> Languages { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Collection> Collections { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Collection>()
            .HasMany(c => c.SubCollections)
            .WithOne(c => c.ParentCollection)
            .HasForeignKey(c => c.ParentCollectionId);

        modelBuilder.Entity<Collection>()
            .HasMany(c => c.Snippets)
            .WithOne(s => s.Collection)
            .HasForeignKey(s => s.CollectionId);
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
            new Tag { Id = 1, Name = "Machine Learning" },
            new Tag { Id = 2, Name = "Automation" },
            new Tag { Id = 3, Name = "Web Scraping" }
        );

        modelBuilder.Entity<Language>().HasData(
            new Language { Id = 1, Name = "Python" },
            new Language { Id = 2, Name = "JavaScript" },
            new Language { Id = 3, Name = "C#" },
            new Language { Id = 4, Name = "Java" },
            new Language { Id = 5, Name = "Ruby" },
            new Language { Id = 6, Name = "Go" },
            new Language { Id = 7, Name = "PHP" },
            new Language { Id = 8, Name = "Swift" },
            new Language { Id = 9, Name = "Kotlin" },
            new Language { Id = 10, Name = "TypeScript" }
        );

        modelBuilder.Entity<Snippet>()
            .HasOne(s => s.Language)
            .WithMany(l => l.Snippets)
            .HasForeignKey(s => s.LanguageId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Snippet>()
            .Property(s => s.LanguageId)
            .HasDefaultValue(1);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.UserName)
            .IsUnique();
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
    }

}