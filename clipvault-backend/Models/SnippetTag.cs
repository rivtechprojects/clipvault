using System.ComponentModel.DataAnnotations.Schema;

namespace ClipVault.Models;

[Table("SnippetTag")]
public class SnippetTag
{
    public int SnippetId { get; set; }
    public Snippet? Snippet { get; set; }
    public int TagId { get; set; }
    public Tag? Tag { get; set ;}
}