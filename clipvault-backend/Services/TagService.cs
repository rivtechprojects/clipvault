using System.Runtime.CompilerServices;
using ClipVault.Interfaces;
using ClipVault.Models;
using Microsoft.EntityFrameworkCore;


public class TagService : ITagService
{
    private readonly IAppDbContext _context;

    public TagService(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Tag>> GetTagsByIdsAsync(List<int> tagIds)
    {
        return await _context.Tags
            .Where(tag => tagIds.Contains(tag.Id))
            .ToListAsync();
    }

    public void UpdateSnippetTags(Snippet snippet, List<int> tagIds)
    {
        var existingTagIds = snippet.SnippetTags.Select(st => st.TagId).ToList();

        // Remove tags that are no longer in the updated list
        var tagsToRemove = snippet.SnippetTags
            .Where(st => !tagIds.Contains(st.TagId))
            .ToList();

        foreach (var tag in tagsToRemove)
        {
            snippet.SnippetTags.Remove(tag);
        }

        // Add new tags that are not already in the collection
        var newTagIds = tagIds.Except(existingTagIds).ToList();
        foreach (var tagId in newTagIds)
        {
            snippet.SnippetTags.Add(new SnippetTag { TagId = tagId });
        }
    }

}
