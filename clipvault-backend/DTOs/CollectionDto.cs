namespace ClipVault.Dtos;

public class CollectionDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int? ParentCollectionId { get; set; }
    public List<CollectionDto>? SubCollections { get; set; }
    public List<SnippetResponseDto>? Snippets { get; set; }
}