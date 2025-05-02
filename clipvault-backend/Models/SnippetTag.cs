public class SnippetTag
{
    public int SnippetId { get; set; }
    public required Snippet Snippet { get; set; }
    public int TagId { get; set; }
    public required Tag Tag { get; set ;}
}