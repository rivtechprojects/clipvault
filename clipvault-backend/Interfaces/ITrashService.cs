using ClipVault.Dtos;

namespace ClipVault.Interfaces
{
    public interface ITrashService
    {
        // Collections
        Task<List<CollectionDto>> GetTrashedCollectionsAsync();
        Task<CollectionDto?> RestoreCollectionAsync(int id);
        Task<int> EmptyTrashAsync();
        Task<bool> DeleteCollectionAsync(int id);

        // Snippets
        Task<List<SnippetResponseDto>> GetTrashedSnippetsAsync();
        Task<SnippetResponseDto?> RestoreSnippetAsync(int id);
        Task<bool> DeleteSnippetAsync(int id);
    }
}
