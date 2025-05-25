using ClipVault.Dtos;

namespace ClipVault.Interfaces;

public interface ICollectionMapper
{
    CollectionDto MapToCollectionDto(Collection collection);
    Collection MapToCollectionEntity(CollectionDto dto);
    Collection MapToCollectionEntity(CollectionCreateDto dto);
}
