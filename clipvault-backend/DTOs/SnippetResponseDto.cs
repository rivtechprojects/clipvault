using System.ComponentModel.DataAnnotations;

namespace ClipVault.Dtos;

public class SnippetResponseDto
{
    public int Id { get; set; }
    [Required]
    public required string Title { get; set; }
    [Required]
    public required string Code { get; set; }
    [Required]
    public required string Language { get; set; }
    public List<string> Tags { get; set; } = new List<string>();
}