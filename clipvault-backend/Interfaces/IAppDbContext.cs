using ClipVault.Models;
using Microsoft.EntityFrameworkCore;
namespace ClipVault.Interfaces;

public interface IAppDbContext
{
    DbSet<Snippet> Snippets { get; }
    DbSet<Tag> Tags { get; }
    DbSet<SnippetTag> SnippetTags { get; }
    DbSet<Language> Languages { get; set; }
    DbSet<User> Users { get; set; }
    DbSet<Collection> Collections { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}