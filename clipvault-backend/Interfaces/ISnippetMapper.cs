using ClipVault.Dtos;
using ClipVault.Models;

namespace ClipVault.Interfaces;

public interface ISnippetMapper
{
    SnippetResponseDto MapToSnippetResponseDto(Snippet snippet);
    Snippet MapToSnippetEntity(SnippetCreateDto snippetDto, int languageId, List<Tag> tags);
}