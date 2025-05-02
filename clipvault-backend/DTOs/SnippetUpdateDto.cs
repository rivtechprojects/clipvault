namespace ClipVault.Dtos;

public class SnippetUpdateDto
{
    public string Title { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public List<int> TagIds { get; set; } = new List<int>();
}