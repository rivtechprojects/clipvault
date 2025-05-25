using ClipVault.Dtos;
using ClipVault.Interfaces;

namespace ClipVault.Mappers;

public class CollectionMapper : ICollectionMapper
{
    private readonly ISnippetMapper _snippetMapper;
    public CollectionMapper(ISnippetMapper snippetMapper)
    {
        _snippetMapper = snippetMapper;
    }

    public CollectionDto MapToCollectionDto(Collection collection)
    {
        return new CollectionDto
        {
            Id = collection.Id,
            Name = collection.Name,
            ParentCollectionId = collection.ParentCollectionId,
            SubCollections = collection.SubCollections?.Select(MapToCollectionDto).ToList(),
            Snippets = collection.Snippets?.Select(_snippetMapper.MapToSnippetResponseDto).ToList()
        };
    }

    public Collection MapToCollectionEntity(CollectionDto dto)
    {
        return new Collection
        {
            Id = dto.Id,
            Name = dto.Name,
            ParentCollectionId = dto.ParentCollectionId
        };
    }

    public Collection MapToCollectionEntity(CollectionCreateDto dto)
    {
        return new Collection
        {
            Name = dto.Name,
            ParentCollectionId = dto.ParentCollectionId
        };
    }
}
