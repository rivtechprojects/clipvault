using ClipVault.Dtos;
using ClipVault.Exceptions;
using ClipVault.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ClipVault.Services;

public class CollectionService : ICollectionService
{
    private readonly IAppDbContext _context;
    private readonly ICollectionMapper _collectionMapper;
    private readonly ISnippetService _snippetService;

    public CollectionService(IAppDbContext context, ICollectionMapper collectionMapper, ISnippetService snippetService)
    {
        _context = context;
        _collectionMapper = collectionMapper;
        _snippetService = snippetService;
    }

    public async Task<CollectionDto> CreateCollectionAsync(CollectionCreateDto collectionCreateDto)
    {
        var collection = _collectionMapper.MapToCollectionEntity(collectionCreateDto);
        _context.Collections.Add(collection);
        await _context.SaveChangesAsync();
        return _collectionMapper.MapToCollectionDto(collection);
    }

    public async Task<CollectionDto> GetCollectionWithSnippetsAsync(int collectionId)
    {
        var collection = await _context.Collections
            .Include(c => c.Snippets)
            .Include(c => c.SubCollections)
            .FirstOrDefaultAsync(c => c.Id == collectionId);
        if (collection == null)
            throw new NotFoundException($"Collection with ID {collectionId} not found.");
            
        return _collectionMapper.MapToCollectionDto(collection);
    }

    public async Task<List<CollectionDto>> GetAllCollectionsAsync()
    {
        var collections = await _context.Collections
            .Where(c => c.ParentCollectionId == null)
            .Include(c => c.Snippets)
            .Include(c => c.SubCollections)
            .ToListAsync();
        return collections.Select(_collectionMapper.MapToCollectionDto).ToList();
    }

    public async Task<CollectionDto> UpdateCollectionAsync(int collectionId, CollectionUpdateDto updateDto)
    {
        var collection = await _context.Collections.FirstOrDefaultAsync(c => c.Id == collectionId);
        if (collection == null)
            throw new NotFoundException($"Collection with ID {collectionId} not found.");
        collection.Name = updateDto.Name;
        await _context.SaveChangesAsync();
        return _collectionMapper.MapToCollectionDto(collection);
    }

    public async Task DeleteCollectionAsync(int collectionId)
    {
        var collection = await _context.Collections
            .Include(c => c.Snippets)
            .Include(c => c.SubCollections)
            .FirstOrDefaultAsync(c => c.Id == collectionId);
        if (collection == null)
            throw new NotFoundException($"Collection with ID {collectionId} not found.");

        // Recursively delete all subcollections
        if (collection.SubCollections != null && collection.SubCollections.Count > 0)
        {
            foreach (var subCollection in collection.SubCollections.ToList())
            {
                await DeleteCollectionAsync(subCollection.Id);
            }
        }

        await _snippetService.DeleteSnippetsByCollectionAsync(collectionId);

        _context.Collections.Remove(collection);
        await _context.SaveChangesAsync();
    }

    public async Task<CollectionDto> MoveCollectionAsync(int childId, int? parentId)
    {
        var child = await _context.Collections.FirstOrDefaultAsync(c => c.Id == childId);
        if (child == null)
            throw new NotFoundException($"Collection with ID {childId} not found.");
        if (parentId.HasValue)
        {
            var parentIdValue = parentId.Value;
            var parent = await _context.Collections.FirstOrDefaultAsync(c => c.Id == parentIdValue);
            if (parent == null)
                throw new NotFoundException($"Parent collection with ID {parentIdValue} not found.");
            child.ParentCollectionId = parentIdValue;
        }
        else
        {
            child.ParentCollectionId = null;
        }
        await _context.SaveChangesAsync();
        return _collectionMapper.MapToCollectionDto(child);
    }
}
