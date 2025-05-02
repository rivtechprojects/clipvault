using ClipVault.Dtos;
using ClipVault.Interfaces;
using ClipVault.Models;
using Microsoft.EntityFrameworkCore;

namespace ClipVault.Services;

public class SnippetService : ISnippetService
{
    private readonly IAppDbContext _context;

    public SnippetService(IAppDbContext context)
    {
        _context = context;
    }

    // Create a new snippet
    public async Task<SnippetResponseDto> CreateSnippetAsync(SnippetCreateDto snippetDto)
    {
        // Validate that all TagIds exist in the database
        var existingTags = await GetTagsByIdsAsync(snippetDto.TagIds);
        if (existingTags.Count != snippetDto.TagIds.Count)
        {
            throw new ArgumentException("One or more TagIds are invalid.");
        }

        // Map the DTO to the Snippet entity
        var snippet = new Snippet
        {
            Title = snippetDto.Title,
            Code = snippetDto.Code,
            Language = snippetDto.Language,
            SnippetTags = snippetDto.TagIds.Select(tagId => new SnippetTag
            {
                TagId = tagId
            }).ToList()
        };

        // Save the snippet to the database
        _context.Snippets.Add(snippet);
        await _context.SaveChangesAsync();

        return MapToSnippetResponseDto(snippet);
    }

    // Get all snippets
    public async Task<List<SnippetResponseDto>> GetAllSnippetsAsync()
    {
        var snippets = await _context.Snippets
        .AsNoTracking()
        .Include(s => s.SnippetTags)
        .ThenInclude(st => st.Tag)
        .ToListAsync();

        return snippets.Select(MapToSnippetResponseDto).ToList();
    }

    public async Task<SnippetResponseDto?> GetSnippetByIdAsync(int id)
    {
        var snippet = await _context.Snippets
        .AsNoTracking()
        .Include(s => s.SnippetTags)
        .ThenInclude(st => st.Tag)
        .FirstOrDefaultAsync(s => s.Id == id);

        if (snippet == null) return null;

        return MapToSnippetResponseDto(snippet);
    }

    public async Task<List<Tag>> GetTagsByIdsAsync(List<int> tagIds)
    {
        return await _context.Tags
            .Where(tag => tagIds.Contains(tag.Id))
            .ToListAsync();
    }

    // Get snippets by tag
    public async Task<List<Snippet>> GetSnippetsByTagAsync(string tagName)
    {
        return await _context.Snippets
            .Include(s => s.SnippetTags)
            .ThenInclude(st => st.Tag)
            .Where(s => s.SnippetTags.Any(st => st.Tag != null && st.Tag.Name == tagName))
            .ToListAsync();
    }

    // Update a snippet
    public async Task<SnippetResponseDto?> UpdateSnippetAsync(int id, SnippetUpdateDto snippetDto)
    {
        var snippet = await _context.Snippets
            .Include(s => s.SnippetTags)
            .ThenInclude(st => st.Tag)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (snippet == null) return null;

        // Update snippet fields
        snippet.Title = snippetDto.Title;
        snippet.Code = snippetDto.Code;
        snippet.Language = snippetDto.Language;

        // Update tags
        await UpdateSnippetTags(snippet, snippetDto.TagIds);

        // Save changes
        await _context.SaveChangesAsync();

        // Reload snippet to ensure related entities are loaded
        snippet = await _context.Snippets
            .Include(s => s.SnippetTags)
            .ThenInclude(st => st.Tag)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (snippet == null) return null;

        // Map to DTO
        return MapToSnippetResponseDto(snippet);
    }

    private Task UpdateSnippetTags(Snippet snippet, List<int> tagIds)
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

        return Task.CompletedTask;
    }

    private static SnippetResponseDto MapToSnippetResponseDto(Snippet snippet)
    {
        return new SnippetResponseDto
        {
            Id = snippet.Id,
            Title = snippet.Title,
            Code = snippet.Code,
            Language = snippet.Language,
            Tags = snippet.SnippetTags.Select(st => new TagDto
            {
                Id = st.TagId,
                Name = st.Tag?.Name ?? "Unknown" // Handle null Tag gracefully
            }).ToList()
        };
    }

    // Delete a snippet
    public async Task<bool> DeleteSnippetAsync(int id)
    {
        var snippet = await _context.Snippets.FindAsync(id);
        if (snippet == null) return false;

        _context.Snippets.Remove(snippet);
        await _context.SaveChangesAsync();
        return true;
    }
}