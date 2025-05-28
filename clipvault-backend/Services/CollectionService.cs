using ClipVault.Dtos;
using ClipVault.Exceptions;
using ClipVault.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ClipVault.Services;

public class CollectionService : ICollectionService
{
    private readonly IAppDbContext _context;
    private readonly ICollectionMapper _collectionMapper;

    public CollectionService(IAppDbContext context, ICollectionMapper collectionMapper)
    {
        _context = context;
        _collectionMapper = collectionMapper;
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
            .Where(c => !c.IsDeleted)
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
            .Where(c => c.ParentCollectionId == null && !c.IsDeleted)
            .Include(c => c.Snippets)
            .Include(c => c.SubCollections)
            .ToListAsync();
        return collections.Select(_collectionMapper.MapToCollectionDto).ToList();
    }

    public async Task<CollectionDto> UpdateCollectionAsync(int collectionId, CollectionUpdateDto updateDto)
    {
        var collection = await _context.Collections.Where(c => !c.IsDeleted).FirstOrDefaultAsync(c => c.Id == collectionId);
        if (collection == null)
            throw new NotFoundException($"Collection with ID {collectionId} not found.");
        collection.Name = updateDto.Name;
        await _context.SaveChangesAsync();
        return _collectionMapper.MapToCollectionDto(collection);
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

    public async Task<bool> SoftDeleteCollectionAsync(int id)
    {
        var collection = await _context.Collections
            .Include(c => c.SubCollections)
            .Include(c => c.Snippets)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (collection == null || collection.IsDeleted)
            return false;
        await SoftDeleteCollectionRecursive(collection);
        await _context.SaveChangesAsync();
        return true;
    }

    private async Task SoftDeleteCollectionRecursive(Collection collection)
    {
        collection.IsDeleted = true;
        if (collection.Snippets != null)
        {
            foreach (var snippet in collection.Snippets)
            {
                snippet.IsDeleted = true;
            }
        }
        if (collection.SubCollections != null)
        {
            foreach (var sub in collection.SubCollections)
            {
                await SoftDeleteCollectionRecursive(sub);
            }
        }
    }
}
