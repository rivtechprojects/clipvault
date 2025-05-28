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
        snippet.CollectionId = snippetDto.CollectionId;

        // Save the snippet to the database
        _context.Snippets.Add(snippet);
        await _context.SaveChangesAsync();

        return _snippetMapper.MapToSnippetResponseDto(snippet);
    }

    // Get all snippets
    public async Task<List<SnippetResponseDto>> GetAllSnippetsAsync()
    {
        var snippets = await _context.Snippets
            .Where(s => !s.IsDeleted)
            .AsNoTracking()
            .Include(s => s.Language)
            .Include(s => s.SnippetTags)
            .ThenInclude(st => st.Tag)
            .ToListAsync();

        return snippets.Select(_snippetMapper.MapToSnippetResponseDto).ToList();
    }

    public async Task<SnippetResponseDto?> GetSnippetByIdAsync(int id)
    {
        var snippet = await _context.Snippets
            .AsNoTracking()
            .Include(s => s.Language)
            .Include(s => s.SnippetTags)
            .ThenInclude(st => st.Tag)
            .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted)
            ?? throw new NotFoundException($"Snippet with ID {id} not found.");

        return _snippetMapper.MapToSnippetResponseDto(snippet);
    }

    // Get snippets by tag
    public async Task<List<SnippetResponseDto>> GetSnippetsDtoByTagAsync(string tagName)
    {
        var snippets = await _context.Snippets
            .Where(s => !s.IsDeleted)
            .Include(s => s.SnippetTags)
            .ThenInclude(st => st.Tag)
            .Where(s => s.SnippetTags.Any(st => st.Tag != null && st.Tag.Name == tagName))
            .ToListAsync();

        // Map to DTOs using SnippetMapper
        return snippets.Select(_snippetMapper.MapToSnippetResponseDto).ToList();
    }
    private async Task<Snippet?> GetSnippetEntityByIdAsync(int id)
    {
        return await _context.Snippets
            .Include(s => s.SnippetTags)
            .ThenInclude(st => st.Tag)
            .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
    }
    public async Task<SnippetResponseDto?> UpdateSnippetAsync(int id, SnippetUpdateDto snippetDto)
    {
        // Retrieve the existing snippet entity
        var existingSnippet = await GetSnippetEntityByIdAsync(id);
        if (existingSnippet == null)
        {
            throw new NotFoundException($"Snippet with ID {id} not found.");
        }

        // Merge the incoming DTO with the existing snippet
        if (snippetDto.Title != null) existingSnippet.Title = snippetDto.Title;
        if (snippetDto.Code != null) existingSnippet.Code = snippetDto.Code;

        // Update the language if provided
        if (!string.IsNullOrEmpty(snippetDto.Language))
        {
            var normalizedLanguage = snippetDto.Language.Trim().ToLower();

            var language = await _context.Languages
                .FirstOrDefaultAsync(l => l.Name.ToLower() == normalizedLanguage);

            if (language == null)
            {
                language = new Language { Name = snippetDto.Language.Trim() };
                _context.Languages.Add(language);
                await _context.SaveChangesAsync();
            }

            existingSnippet.LanguageId = language.Id;
        }

        // update collectionid if provided
        if (snippetDto.CollectionId.HasValue)
        {
            existingSnippet.CollectionId = snippetDto.CollectionId.Value;
        }

        // Save changes to the database
        await _context.SaveChangesAsync();

        // Return the updated snippet as a DTO
        return _snippetMapper.MapToSnippetResponseDto(existingSnippet);
    }

    public async Task<SnippetResponseDto?> AddTagsToSnippetAsync(int snippetId, List<string> tagNames)
    {
        var snippet = await GetSnippetEntityByIdAsync(snippetId);
        if (snippet == null)
        {
            throw new NotFoundException($"Snippet with ID {snippetId} not found.");
        }

        var existingTags = await _tagService.ValidateAndCreateTagsAsync(tagNames);

        var tagsToAdd = existingTags
            .Where(tag => !snippet.SnippetTags.Any(st => st.TagId == tag.Id))
            .Select(tag => new SnippetTag { TagId = tag.Id });

        snippet.SnippetTags.AddRange(tagsToAdd);

        await _context.SaveChangesAsync();

        // Return the updated snippet as a DTO
        return _snippetMapper.MapToSnippetResponseDto(snippet);
    }

    public async Task RemoveTagsFromSnippetAsync(int snippetId, List<string> tagNames)
    {
        var snippet = await GetSnippetEntityByIdAsync(snippetId);
        if (snippet == null)
        {
            throw new NotFoundException($"Snippet with ID {snippetId} not found.");
        }

        var tagsToRemove = snippet.SnippetTags
            .Where(st => tagNames
                .Select(t => t.ToLower())
                .Contains(st.Tag?.Name?.ToLower()))
            .ToList();

        foreach (var tagToRemove in tagsToRemove)
        {
            snippet.SnippetTags.Remove(tagToRemove);
        }

        await _context.SaveChangesAsync();
    }

    public async Task ReplaceTagsForSnippetAsync(int snippetId, List<string> tagNames)
    {
        var snippet = await GetSnippetEntityByIdAsync(snippetId);
        if (snippet == null)
        {
            throw new NotFoundException($"Snippet with ID {snippetId} not found.");
        }

        // Clear existing tags
        snippet.SnippetTags.Clear();

        // Add new tags
        var newTags = await _tagService.ValidateAndCreateTagsAsync(tagNames);
        snippet.SnippetTags = newTags.Select(tag => new SnippetTag { TagId = tag.Id }).ToList();

        await _context.SaveChangesAsync();
    }

    public async Task<bool> SoftDeleteSnippetAsync(int id)
    {
        // Only soft delete if not already deleted
        var snippet = await _context.Snippets.FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
        if (snippet == null)
            return false;
        snippet.IsDeleted = true;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<SnippetResponseDto>> SearchSnippetsAsync(string? keyword, string? language, List<string>? tagNames)
    {
        var query = _context.Snippets
            .Where(s => !s.IsDeleted)
            .Include(s => s.SnippetTags)
            .ThenInclude(st => st.Tag)
            .Include(s => s.Language)
            .AsQueryable();

        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(s => s.Title.Contains(keyword) || s.Code.Contains(keyword));
        }

        if (!string.IsNullOrEmpty(language))
        {
            var normalizedLanguage = language.ToLower();
            query = query.Where(s => s.Language.Name.ToLower() == normalizedLanguage);
        }

        if (tagNames != null && tagNames.Count > 0)
        {
            var normalizedTagNames = tagNames.Select(t => t.ToLower()).ToList();
            query = query.Where(s => s.SnippetTags.Any(st => st.Tag != null && normalizedTagNames.Contains(st.Tag.Name.ToLower())));
        }

        var snippets = await query.ToListAsync();
        return snippets.Select(_snippetMapper.MapToSnippetResponseDto).ToList();
    }
}