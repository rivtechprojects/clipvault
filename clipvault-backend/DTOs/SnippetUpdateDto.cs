namespace ClipVault.Dtos;

public class SnippetUpdateDto
{
    public string Title { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public List<string> TagNames { get; set; } = new List<string>();
}