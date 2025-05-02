public class Tag
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public ICollection<SnippetTag> SnippetTags { get; set; } = new List<SnippetTag>();
}
