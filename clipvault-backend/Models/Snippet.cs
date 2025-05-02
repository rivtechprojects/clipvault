using System.ComponentModel.DataAnnotations.Schema;

namespace ClipVault.Models;

[Table("Snippet")]
public class Snippet
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Code { get; set; }
    public required string Language { get; set; }
    public List<SnippetTag> SnippetTags { get; set; } = new List<SnippetTag>();
}
