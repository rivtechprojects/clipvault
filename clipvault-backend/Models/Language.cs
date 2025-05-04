using System.ComponentModel.DataAnnotations.Schema;

namespace ClipVault.Models;

[Table("Language")]
public class Language
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<Snippet> Snippets { get; set; } = new List<Snippet>();
}