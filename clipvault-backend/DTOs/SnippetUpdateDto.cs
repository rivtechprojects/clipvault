namespace ClipVault.Dtos;

public class SnippetUpdateDto
{
    public string? Title { get; set; }
    public string? Code { get; set; }
    public string? Language { get; set; }
    public List<string>? TagNames { get; set; }
    public int? CollectionId { get; set; }
}