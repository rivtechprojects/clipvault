using ClipVault.Dtos;
using ClipVault.Interfaces;
using ClipVault.Models;


public class SnippetMapper : ISnippetMapper
{
    public SnippetResponseDto MapToSnippetResponseDto(Snippet snippet)
    {
        return new SnippetResponseDto
        {
            Id = snippet.Id,
            Title = snippet.Title,
            Code = snippet.Code,
            Language = snippet.Language,
            Tags = snippet.SnippetTags.Select(st => new TagDto
            {
                Id = st.TagId,
                Name = st.Tag?.Name ?? "Unknown"
            }).ToList()
        };
    }
}