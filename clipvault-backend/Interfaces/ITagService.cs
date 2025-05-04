using ClipVault.Models;

namespace ClipVault.Interfaces;

public interface ITagService
{
    Task<List<Tag>> GetTagsByIdsAsync(List<int> tagIds);
    void UpdateSnippetTags(Snippet snippet, List<int> tagIds);
    Task<List<Tag>> GetTagsByNamesAsync(List<string> tagNames);
    Task<List<Tag>> ValidateAndCreateTagsAsync(List<string> tagNames);

}