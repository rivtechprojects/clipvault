using ClipVault.Dtos;
using ClipVault.Interfaces;
using ClipVault.Models;
using Microsoft.EntityFrameworkCore;

namespace ClipVault.Services;

public class SnippetService : ISnippetService
{
    private readonly IAppDbContext _context;
    private readonly ITagService _tagService;
    private readonly ISnippetMapper _snippetMapper;

    public SnippetService(IAppDbContext context, ITagService tagService, ISnippetMapper snippetMapper)
    {
        _context = context;
        _tagService = tagService;
        _snippetMapper = snippetMapper;
    }

    // Create a new snippet
    public async Task<SnippetResponseDto> CreateSnippetAsync(SnippetCreateDto snippetDto)
    {
        // Validate that all TagIds exist in the database
        var existingTags = await _tagService.GetTagsByIdsAsync(snippetDto.TagIds);
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

        return _snippetMapper.MapToSnippetResponseDto(snippet);
    }

    // Get all snippets
    public async Task<List<SnippetResponseDto>> GetAllSnippetsAsync()
    {
        var snippets = await _context.Snippets
        .AsNoTracking()
        .Include(s => s.SnippetTags)
        .ThenInclude(st => st.Tag)
        .ToListAsync();

        return snippets.Select(_snippetMapper.MapToSnippetResponseDto).ToList();
    }

    public async Task<SnippetResponseDto?> GetSnippetByIdAsync(int id)
    {
        var snippet = await _context.Snippets
        .AsNoTracking()
        .Include(s => s.SnippetTags)
        .ThenInclude(st => st.Tag)
        .FirstOrDefaultAsync(s => s.Id == id);

        if (snippet == null) return null;

        return _snippetMapper.MapToSnippetResponseDto(snippet);
    }

    // Get snippets by tag
    public async Task<List<SnippetResponseDto>> GetSnippetsByTagAsync(string tagName)
    {
        var snippets = await _context.Snippets
            .Include(s => s.SnippetTags)
            .ThenInclude(st => st.Tag)
            .Where(s => s.SnippetTags.Any(st => st.Tag != null && st.Tag.Name == tagName))
            .ToListAsync();

        // Map to DTOs using SnippetMapper
        return snippets.Select(_snippetMapper.MapToSnippetResponseDto).ToList();
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
        _tagService.UpdateSnippetTags(snippet, snippetDto.TagIds);

        // Save changes
        await _context.SaveChangesAsync();

        // Reload snippet to ensure related entities are loaded
        snippet = await _context.Snippets
            .Include(s => s.SnippetTags)
            .ThenInclude(st => st.Tag)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (snippet == null) return null;

        // Map to DTO
        return _snippetMapper.MapToSnippetResponseDto(snippet);
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