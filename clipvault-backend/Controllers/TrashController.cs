using ClipVault.Exceptions;
using ClipVault.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clipvault.Controllers;

[ApiController]
[Route("api/trash")]
[Authorize]
public class TrashController : ControllerBase
{
    private readonly ITrashService _trashService;

    public TrashController(ITrashService trashService)
    {
        _trashService = trashService;
    }

    // --- Collections Trash ---
    [HttpGet("collections")]
    public async Task<IActionResult> GetTrashedCollections()
    {
        var trashed = await _trashService.GetTrashedCollectionsAsync();
        return Ok(trashed);
    }

    [HttpPost("collections/{id}/restore")]
    public async Task<IActionResult> RestoreCollection(int id)
    {
        var restored = await _trashService.RestoreCollectionAsync(id);
        if (restored == null)
            throw new NotFoundException($"Collection with ID {id} not found or not trashed.");
        return Ok(restored);
    }

    [HttpDelete("collections/{id}")]
    public async Task<IActionResult> DeleteCollection(int id)
    {
        var success = await _trashService.DeleteCollectionAsync(id);
        if (!success)
            throw new NotFoundException($"Collection with ID {id} not found or not trashed.");
        return NoContent();
    }

    // --- Snippets Trash ---
    [HttpDelete("snippets/{id}")]
    public async Task<IActionResult> DeleteSnippet(int id)
    {
        var success = await _trashService.DeleteSnippetAsync(id);
        if (!success)
        {
            throw new NotFoundException($"Snippet with ID {id} not found.");
        }
        return NoContent();
    }

    [HttpGet("snippets")]
    public async Task<IActionResult> GetTrashedSnippets()
    {
        var trashed = await _trashService.GetTrashedSnippetsAsync();
        return Ok(trashed);
    }

    [HttpPost("snippets/{id}/restore")]
    public async Task<IActionResult> RestoreSnippet(int id)
    {
        var restored = await _trashService.RestoreSnippetAsync(id);
        if (restored == null)
            throw new NotFoundException($"Snippet with ID {id} not found or not trashed.");
        return Ok(restored);
    }

    // --- Empty Trash ---
    [HttpDelete("permanent")]
    public async Task<IActionResult> EmptyTrash()
    {
        var count = await _trashService.EmptyTrashAsync();
        return Ok(new { deleted = count });
    }
}
