using ClipVault.Dtos;
using ClipVault.Interfaces;
using ClipVault.Models;
using Microsoft.AspNetCore.Mvc;

namespace ClipVault.Controllers;

[ApiController]
[Route("api/snippets")]
public class SnippetsController : ControllerBase
{
    private readonly ISnippetService _snippetService;

    public SnippetsController(ISnippetService snippetService)
    {
        _snippetService = snippetService;
    }

    [HttpPost]
    [HttpPost]
    public async Task<IActionResult> CreateSnippet([FromBody] SnippetCreateDto snippetDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Call the service to create the snippet
        var response = await _snippetService.CreateSnippetAsync(snippetDto);

        // Return the created snippet as a DTO
        return CreatedAtAction(nameof(GetSnippetById), new { id = response.Id }, response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllSnippets()
    {
        var snippets = await _snippetService.GetAllSnippetsAsync();
        return Ok(snippets); // Return the DTOs provided by the service
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSnippetById(int id)
    {
        var snippet = await _snippetService.GetSnippetByIdAsync(id);
        if (snippet == null) return NotFound();

        // No need to map SnippetTags here, as the service already returns a SnippetResponseDto
        return Ok(snippet);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSnippet(int id, [FromBody] SnippetUpdateDto snippetDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var snippet = await _snippetService.UpdateSnippetAsync(id, snippetDto);
        if (snippet == null) return NotFound();

        return Ok(snippet);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSnippet(int id)
    {
        var success = await _snippetService.DeleteSnippetAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
}