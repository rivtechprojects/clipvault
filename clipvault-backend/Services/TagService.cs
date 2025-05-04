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

    public async Task<List<Tag>> GetTagsByNamesAsync(List<string> tagNames)
    {
        var lowerCaseTagNames = tagNames.Select(t => t.ToLower()).ToList();
        return await _context.Tags
            .Where(t => lowerCaseTagNames.Contains(t.Name.ToLower()))
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

    public async Task<List<Tag>> ValidateAndCreateTagsAsync(List<string> tagNames)
    {
        // Normalize tag names to lowercase
        var lowerCaseTagNames = tagNames.Select(t => t.ToLower()).ToList();

        // Get existing tags
        var existingTags = await _context.Tags
            .Where(t => lowerCaseTagNames.Contains(t.Name.ToLower()))
            .ToListAsync();

        // Identify new tags that need to be created
        var newTags = tagNames
            .Where(tagName => !existingTags.Any(t => t.Name.ToLower() == tagName.ToLower()))
            .Select(tagName => new Tag { Name = tagName })
            .ToList();

        // Add new tags to the database
        if (newTags.Any())
        {
            _context.Tags.AddRange(newTags);
            await _context.SaveChangesAsync();
            existingTags.AddRange(newTags);
        }

        return existingTags;
    }

}
