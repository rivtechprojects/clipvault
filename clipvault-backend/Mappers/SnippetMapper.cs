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
            Language = snippet.Language?.Name ?? "Unknown",
            Tags = snippet.SnippetTags.Select(st => new TagDto
            {
                Id = st.TagId,
                Name = st.Tag?.Name ?? "Unknown"
            }).ToList()
        };
    }

    // Map SnippetCreateDto to Snippet entity
    public Snippet MapToSnippetEntity(SnippetCreateDto snippetDto, int languageId, List<Tag> tags)
    {
        return new Snippet
        {
            Title = snippetDto.Title,
            Code = snippetDto.Code,
            LanguageId = languageId,
            SnippetTags = tags.Select(tag => new SnippetTag
            {
                TagId = tag.Id
            }).ToList()
        };
    }
}