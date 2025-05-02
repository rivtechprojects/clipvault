using ClipVault.Dtos;
using ClipVault.Models;
namespace ClipVault.Interfaces;

public interface ISnippetService
{
    Task<SnippetResponseDto> CreateSnippetAsync(SnippetCreateDto snippetDto);
    Task<bool> DeleteSnippetAsync(int id);
    Task<List<SnippetResponseDto>> GetAllSnippetsAsync();
    Task<SnippetResponseDto?> GetSnippetByIdAsync(int id);
    Task<List<Tag>> GetTagsByIdsAsync(List<int> tagIds);
    Task<List<Snippet>> GetSnippetsByTagAsync(string tagName);
    Task<SnippetResponseDto?> UpdateSnippetAsync(int id, SnippetUpdateDto snippetDto);
}
