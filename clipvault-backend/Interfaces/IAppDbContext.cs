using ClipVault.Models;
using Microsoft.EntityFrameworkCore;

public interface IAppDbContext
{
    DbSet<Snippet> Snippets { get; }
    DbSet<Tag> Tags { get; }
    DbSet<SnippetTag> SnippetTags { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}