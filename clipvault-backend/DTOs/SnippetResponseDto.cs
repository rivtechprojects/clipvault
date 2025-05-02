namespace ClipVault.Dtos;

public class SnippetResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Code { get; set; }
    public string Language { get; set; }
    public List<TagDto> Tags { get; set; } = new List<TagDto>();
}