using ClipVault.Models;

namespace ClipVault.Interfaces;

public interface ITagService
{
    Task<List<Tag>> GetTagsByIdsAsync(List<int> tagIds);
    void UpdateSnippetTags(Snippet snippet, List<int> tagIds);

}