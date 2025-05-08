using System.ComponentModel.DataAnnotations;

namespace ClipVault.Dtos;

public class TagDto
{
    public int Id { get; set; }
    [Required]
    public required string Name { get; set; }
}