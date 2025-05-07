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
            Language = snippet.Language?.Name ?? "",
            Tags = snippet.SnippetTags.Select(st => st.Tag?.Name ?? "").ToList() // Map to list of tag names
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

    public SnippetUpdateDto MapToUpdateDto(SnippetResponseDto snippetResponse)
    {
        return new SnippetUpdateDto
        {
            Title = snippetResponse.Title,
            Code = snippetResponse.Code,
            Language = snippetResponse.Language,
            TagNames = snippetResponse.Tags
        };
    }
}