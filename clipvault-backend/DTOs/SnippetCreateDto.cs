using System.ComponentModel.DataAnnotations;

namespace ClipVault.Dtos;

public class SnippetCreateDto
{
    [Required(ErrorMessage = "Title is required.")]
    [MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
    public required string Title { get; set; }

    [Required(ErrorMessage = "Code is required.")]
    public required string Code { get; set; }

    [Required(ErrorMessage = "Language is required.")]
    public required string Language { get; set; }

    [Required(ErrorMessage = "Tags cannot be empty.")]
    [MinLength(1, ErrorMessage = "At least one tag is required.")]
    [MaxLength(50, ErrorMessage = "Each tag cannot exceed 50 characters.")]
    public List<string> TagNames { get; set; } = new List<string>();
    
    [Required(ErrorMessage = "CollectionId is required.")]
    public int CollectionId { get; set; }
}