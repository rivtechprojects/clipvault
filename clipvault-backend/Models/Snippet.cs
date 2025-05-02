public class Snippet
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Code { get; set; }
    public required string Language { get; set; }
    public ICollection<SnippetTag> SnippetTags { get; set; } = new List<SnippetTag>();
}
