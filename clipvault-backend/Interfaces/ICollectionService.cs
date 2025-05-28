using ClipVault.Dtos;

namespace ClipVault.Interfaces;

public interface ICollectionService
{
    Task<CollectionDto> CreateCollectionAsync(CollectionCreateDto collectionCreateDto);
    Task<CollectionDto> GetCollectionWithSnippetsAsync(int collectionId);
    Task<List<CollectionDto>> GetAllCollectionsAsync();
    Task<CollectionDto> UpdateCollectionAsync(int collectionId, CollectionUpdateDto updateDto);
    Task<CollectionDto> MoveCollectionAsync(int childId, int? parentId);
    Task<bool> SoftDeleteCollectionAsync(int id);
}
