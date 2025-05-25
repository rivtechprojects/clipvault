using ClipVault.Dtos;
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

    [HttpPost("/api/collections")]
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

    [HttpGet("/api/collections")]
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

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCollection(int id)
    {
        await _collectionService.DeleteCollectionAsync(id);
        return NoContent();
    }
}