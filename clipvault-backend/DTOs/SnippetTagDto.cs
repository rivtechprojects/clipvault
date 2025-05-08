using System.ComponentModel.DataAnnotations;

namespace ClipVault.Dtos;

public class SnippetTagDto
{
    [Required]
    public int SnippetId { get; set; }
    [Required]
    public int TagId { get; set; }
}