using ClipVault.Dtos;
using ClipVault.Exceptions;
using ClipVault.Interfaces;
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
    public async Task<IActionResult> CreateSnippet([FromBody] SnippetCreateDto snippetDto)
    {        
        var response = await _snippetService.CreateSnippetAsync(snippetDto);

        return CreatedAtAction(nameof(GetSnippetById), new { id = response.Id }, response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllSnippets()
    {
        var snippets = await _snippetService.GetAllSnippetsAsync();
        return Ok(snippets);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSnippetById(int id)
    {
        var snippet = await _snippetService.GetSnippetByIdAsync(id) 
            ?? throw new NotFoundException($"Snippet with ID {id} not found.");
            
        return Ok(snippet);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSnippet(int id, [FromBody] SnippetUpdateDto snippetDto)
    {
        var snippet = await _snippetService.UpdateSnippetAsync(id, snippetDto)
            ?? throw new NotFoundException($"Snippet with ID {id} not found.");

        return Ok(snippet);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSnippet(int id)
    {
        var success = await _snippetService.DeleteSnippetAsync(id);
        if (!success) {
            throw new NotFoundException($"Snippet with ID {id} not found.");
        }

        return NoContent();
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchSnippets([FromQuery] string? keyword,[FromQuery] string? language, [FromQuery] List<string>? tagNames)
    {
        var snippets = await _snippetService.SearchSnippetsAsync(keyword, language, tagNames);

        return Ok(snippets);
    }
}