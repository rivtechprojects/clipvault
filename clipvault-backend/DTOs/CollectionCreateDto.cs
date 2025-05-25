namespace ClipVault.Dtos;

public class CollectionCreateDto
{
    public required string Name { get; set; }
    public int? ParentCollectionId { get; set; }
}
