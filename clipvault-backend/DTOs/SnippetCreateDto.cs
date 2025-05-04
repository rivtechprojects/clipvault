namespace ClipVault.Dtos;

public class SnippetCreateDto
{
    public required string Title { get; set; }
    public required string Code { get; set; }
    public required string Language { get; set; }
    public List<string> TagNames { get; set; } = new List<string>();
}