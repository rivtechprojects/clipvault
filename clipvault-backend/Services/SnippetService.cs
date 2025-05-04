using ClipVault.Dtos;
using ClipVault.Exceptions;
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
        // Validate or add the language
        var language = await _context.Languages
            .FirstOrDefaultAsync(l => l.Name.ToLower() == snippetDto.Language.ToLower());

        if (language == null)
        {
            // Create the language if it doesn't exist
            language = new Language { Name = snippetDto.Language };
            _context.Languages.Add(language);
            await _context.SaveChangesAsync();
        }

        // Validate and/or create tags
        var existingTags = await _tagService.ValidateAndCreateTagsAsync(snippetDto.TagNames);

        // Use the SnippetMapper to map the DTO to the Snippet entity
        var snippet = _snippetMapper.MapToSnippetEntity(snippetDto, language.Id, existingTags);

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
        .FirstOrDefaultAsync(s => s.Id == id)
            ?? throw new NotFoundException($"Snippet with ID {id} not found.");

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
            .FirstOrDefaultAsync(s => s.Id == id)
                ?? throw new NotFoundException($"Snippet with ID {id} not found.");

        // Validate or add the language
        var language = await _context.Languages
            .FirstOrDefaultAsync(l => l.Name.ToLower() == snippetDto.Language.ToLower());

        if (language == null)
        {
            language = new Language { Name = snippetDto.Language };
            _context.Languages.Add(language);
            await _context.SaveChangesAsync();
        }

        // Validate and/or create tags
        var existingTags = await _tagService.ValidateAndCreateTagsAsync(snippetDto.TagNames);

        // Update snippet fields
        snippet.Title = snippetDto.Title;
        snippet.Code = snippetDto.Code;
        snippet.LanguageId = language.Id; // Update the LanguageId foreign key

        // Update tags
        _tagService.UpdateSnippetTags(snippet, existingTags.Select(t => t.Id).ToList());

        // Save changes
        await _context.SaveChangesAsync();

        return _snippetMapper.MapToSnippetResponseDto(snippet);
    }

    // Delete a snippet
    public async Task<bool> DeleteSnippetAsync(int id)
    {
        var snippet = await _context.Snippets.FindAsync(id)
            ?? throw new NotFoundException($"Snippet with ID {id} not found.");
        _context.Snippets.Remove(snippet);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<SnippetResponseDto>> SearchSnippetsAsync(string? keyword, string? language, List<string>? tagNames)
    {
        var query = _context.Snippets
            .Include(s => s.SnippetTags)
            .ThenInclude(st => st.Tag)
            .AsQueryable();

        // Search for keyword in the title or code body of the snippet
        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(s => s.Title.Contains(keyword) || s.Code.Contains(keyword));
        }

        // Filter snippets by exact language match (case-insensitive)
        if (!string.IsNullOrEmpty(language))
        {
            var normalizedLanguage = language.ToLower();
            query = query.Where(s => s.Language.Name.ToLower() == normalizedLanguage);
        }

        // Filter snippets to match all provided tags (case-insensitive)
        if (tagNames != null && tagNames.Count > 0)
        {
            var normalizedTagNames = tagNames.Select(t => t.ToLower()).ToList();
            query = query.Where(s => normalizedTagNames.All(tagName =>
                s.SnippetTags.Any(st => st.Tag != null && st.Tag.Name.ToLower() == tagName)));
        }

        var snippets = await query.ToListAsync();
        return snippets.Select(_snippetMapper.MapToSnippetResponseDto).ToList();
    }
}