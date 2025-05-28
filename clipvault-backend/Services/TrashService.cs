using ClipVault.Dtos;
using ClipVault.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ClipVault.Services
{
    public class TrashService : ITrashService
    {
        private readonly IAppDbContext _context;
        private readonly ICollectionMapper _collectionMapper;
        private readonly ISnippetMapper _snippetMapper;

        public TrashService(IAppDbContext context, ICollectionMapper collectionMapper, ISnippetMapper snippetMapper)
        {
            _context = context;
            _collectionMapper = collectionMapper;
            _snippetMapper = snippetMapper;
        }

        // Collections
        public async Task<List<CollectionDto>> GetTrashedCollectionsAsync()
        {
            var trashed = await _context.Collections
                .Where(c => c.ParentCollectionId == null && c.IsDeleted)
                .Include(c => c.Snippets)
                .Include(c => c.SubCollections)
                .ToListAsync();
            return trashed.Select(_collectionMapper.MapToCollectionDto).ToList();
        }

        public async Task<CollectionDto?> RestoreCollectionAsync(int id)
        {
            var collection = await _context.Collections
                .Include(c => c.SubCollections)
                .Include(c => c.Snippets)
                .FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted);
            if (collection == null)
                return null;
            RestoreCollectionRecursive(collection);
            await _context.SaveChangesAsync();
            return _collectionMapper.MapToCollectionDto(collection);
        }

        private static void RestoreCollectionRecursive(Collection collection)
        {
            collection.IsDeleted = false;
            if (collection.Snippets != null)
            {
                foreach (var snippet in collection.Snippets)
                {
                    snippet.IsDeleted = false;
                }
            }
            if (collection.SubCollections != null && collection.SubCollections.Count > 0)
            {
                foreach (var subCollection in collection.SubCollections)
                {
                    RestoreCollectionRecursive(subCollection);
                }
            }
        }

        public async Task<bool> DeleteCollectionAsync(int id)
        {
            var collection = await _context.Collections
                .Include(c => c.SubCollections)
                .Include(c => c.Snippets)
                .FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted);
            if (collection == null)
                return false;
            await DeleteCollectionRecursive(collection);
            await _context.SaveChangesAsync();
            return true;
        }

        private async Task DeleteCollectionRecursive(Collection collection)
        {
            // Recursively delete subcollections
            if (collection.SubCollections != null && collection.SubCollections.Count > 0)
            {
                foreach (var sub in collection.SubCollections.ToList())
                {
                    await DeleteCollectionRecursive(sub);
                }
            }
            // Remove all snippets in this collection
            if (collection.Snippets != null && collection.Snippets.Count > 0)
            {
                _context.Snippets.RemoveRange(collection.Snippets);
            }
            _context.Collections.Remove(collection);
        }

        public async Task<int> EmptyTrashAsync()
        {
            var trashedCollections = await _context.Collections
                .Where(c => c.IsDeleted)
                .ToListAsync();
            foreach (var collection in trashedCollections)
            {
                _context.Collections.Remove(collection);
            }
            var trashedSnippets = await _context.Snippets
                .Where(s => s.IsDeleted && (s.Collection == null || !s.Collection.IsDeleted))
                .ToListAsync();
            foreach (var snippet in trashedSnippets)
            {
                _context.Snippets.Remove(snippet);
            }
            var count = trashedCollections.Count + trashedSnippets.Count;
            await _context.SaveChangesAsync();
            return count;
        }

        // Snippets
        public async Task<List<SnippetResponseDto>> GetTrashedSnippetsAsync()
        {
            var trashed = await _context.Snippets
                .Where(s => s.IsDeleted && (s.Collection == null || !s.Collection.IsDeleted))
                .Include(s => s.Language)
                .Include(s => s.SnippetTags)
                .ThenInclude(st => st.Tag)
                .ToListAsync();
            return trashed.Select(_snippetMapper.MapToSnippetResponseDto).ToList();
        }

        public async Task<SnippetResponseDto?> RestoreSnippetAsync(int id)
        {
            var snippet = await _context.Snippets.FirstOrDefaultAsync(s => s.Id == id && s.IsDeleted);
            if (snippet == null)
                return null;
            snippet.IsDeleted = false;
            await _context.SaveChangesAsync();
            return _snippetMapper.MapToSnippetResponseDto(snippet);
        }

        public async Task<bool> DeleteSnippetAsync(int id)
        {
            var snippet = await _context.Snippets.FirstOrDefaultAsync(s => s.Id == id && s.IsDeleted);
            if (snippet == null)
                return false;
            _context.Snippets.Remove(snippet);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
