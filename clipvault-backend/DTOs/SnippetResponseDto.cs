namespace ClipVault.Dtos;

public class SnippetResponseDto
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Code { get; set; }
    public required string Language { get; set; }
    public List<TagDto> Tags { get; set; } = new List<TagDto>();
}