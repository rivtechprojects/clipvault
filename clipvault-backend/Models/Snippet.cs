using System.ComponentModel.DataAnnotations.Schema;

namespace ClipVault.Models;

[Table("Snippet")]
public class Snippet
{
    public int Id { get; set; } // Primary key
    public int? CollectionId { get; set; } // Foreign key, now nullable
    public Collection? Collection { get; set;  } // nav property for collection
    public required string Title { get; set; } // title or name of snippet
    public required string Code { get; set; } // code body
    public int LanguageId { get; set; }  // foreign key referencing language table
    public Language Language { get; set; } = null!; // nav property for language 
    public List<SnippetTag> SnippetTags { get; set; } = new List<SnippetTag>(); // nav property for tags
}
