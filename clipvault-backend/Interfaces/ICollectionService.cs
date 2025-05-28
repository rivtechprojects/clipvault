using ClipVault.Dtos;
using System.Threading.Tasks;

namespace ClipVault.Interfaces;

public interface ICollectionService
{
    Task<CollectionDto> CreateCollectionAsync(CollectionCreateDto collectionCreateDto);
    Task<CollectionDto> GetCollectionWithSnippetsAsync(int collectionId);
    Task<List<CollectionDto>> GetAllCollectionsAsync();
    Task<CollectionDto> UpdateCollectionAsync(int collectionId, CollectionUpdateDto updateDto);
    Task DeleteCollectionAsync(int collectionId);
    Task<CollectionDto> MoveCollectionAsync(int childId, int? parentId);
}
