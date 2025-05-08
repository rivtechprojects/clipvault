using System.ComponentModel.DataAnnotations;

namespace ClipVault.Dtos;

public class SnippetCreateDto
{
    [Required]
    public required string Title { get; set; }
    [Required]
    public required string Code { get; set; }
    [Required]
    public required string Language { get; set; }
    public List<string> TagNames { get; set; } = new List<string>();
}