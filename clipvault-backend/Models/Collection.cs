using ClipVault.Models;

public class Collection
{
    public int Id { get; set; } // primary key
    public required string Name { get; set; }
    public ICollection<Snippet> Snippets { get; set; } = new List<Snippet>();
    public int? ParentCollectionId { get; set; }
    public Collection? ParentCollection { get; set; }
    public ICollection<Collection> SubCollections { get; set; } = new List<Collection>();
    
}