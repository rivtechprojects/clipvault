using ClipVault.Dtos;
using ClipVault.Exceptions;
using ClipVault.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clipvault.Controllers;

[ApiController]
[Route("api/collections")]
[Authorize]
public class CollectionsController : ControllerBase
{
    private readonly ICollectionService _collectionService;

    public CollectionsController(ICollectionService collectionService)
    {
        _collectionService = collectionService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateCollection([FromBody] CollectionCreateDto collectionCreateDto)
    {
        var result = await _collectionService.CreateCollectionAsync(collectionCreateDto);
        return CreatedAtAction(nameof(GetCollectionById), new { id = result.Id }, result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCollectionById(int id)
    {
        var result = await _collectionService.GetCollectionWithSnippetsAsync(id);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCollections()
    {
        var result = await _collectionService.GetAllCollectionsAsync();
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCollection(int id, [FromBody] CollectionUpdateDto dto)
    {
        var updated = await _collectionService.UpdateCollectionAsync(id, dto);
        return Ok(updated);
    }

    [HttpPut("{childId}/parent")]
    public async Task<IActionResult> MoveCollection(int childId, [FromQuery] int? parentId)
    {
        var updated = await _collectionService.MoveCollectionAsync(childId, parentId);
        return Ok(updated);
    }

    [HttpDelete("{id}/trash")]
    public async Task<IActionResult> SoftDeleteCollection(int id)
    {
        var success = await _collectionService.SoftDeleteCollectionAsync(id);
        if (!success)
            throw new NotFoundException($"Collection with ID {id} not found.");
        return NoContent();
    }
}