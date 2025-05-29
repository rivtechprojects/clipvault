using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using ClipVault.Controllers;
using ClipVault.Interfaces;
using ClipVault.Dtos;
using ClipVault.Exceptions;
using ClipVault.Tests.Mocks;

namespace ClipVault.Tests.Controllers
{
    public class SnippetsControllerTests
    {
        private readonly Mock<ISnippetService> _snippetServiceMock;
        private readonly SnippetsController _snippetsController;

        public SnippetsControllerTests()
        {
            _snippetServiceMock = new Mock<ISnippetService>();
            _snippetsController = new SnippetsController(_snippetServiceMock.Object);
        }

        [Fact]
        public async Task CreateSnippet_ValidRequest_ReturnsCreatedAtAction()
        {
            // Arrange
            var snippetDto = MockDataFactory.CreateSnippetCreateDto();
            var snippetResponse = MockDataFactory.CreateSnippetResponseDto();

            _snippetServiceMock.Setup(s => s.CreateSnippetAsync(snippetDto)).ReturnsAsync(snippetResponse);

            // Act
            var result = await _snippetsController.CreateSnippet(snippetDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(_snippetsController.GetSnippetById), createdAtActionResult.ActionName);
            Assert.Equal(snippetResponse, createdAtActionResult.Value);
        }

        [Fact]
        public async Task GetAllSnippets_ReturnsOkWithSnippets()
        {
            // Arrange
            var snippets = new List<SnippetResponseDto> { MockDataFactory.CreateSnippetResponseDto() };
            _snippetServiceMock.Setup(s => s.GetAllSnippetsAsync()).ReturnsAsync(snippets);

            // Act
            var result = await _snippetsController.GetAllSnippets();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(snippets, okResult.Value);
        }

        [Fact]
        public async Task GetSnippetById_ValidId_ReturnsOkWithSnippet()
        {
            // Arrange
            var snippet = MockDataFactory.CreateSnippetResponseDto();
            _snippetServiceMock.Setup(s => s.GetSnippetByIdAsync(1)).ReturnsAsync(snippet);

            // Act
            var result = await _snippetsController.GetSnippetById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(snippet, okResult.Value);
        }

        [Fact]
        public async Task GetSnippetById_InvalidId_ThrowsNotFoundException()
        {
            // Arrange
            _snippetServiceMock.Setup(s => s.GetSnippetByIdAsync(1)).ReturnsAsync((SnippetResponseDto?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _snippetsController.GetSnippetById(1));
        }

        [Fact]
        public async Task UpdateSnippet_ValidId_ReturnsUpdatedSnippet()
        {
            // Arrange
            var snippetDto = new SnippetUpdateDto { /* Add properties */ };
            var updatedSnippet = new SnippetResponseDto { Id = 1, Title = "Updated Title", Code = "Updated Code", Language = "C#" };
            _snippetServiceMock.Setup(s => s.UpdateSnippetAsync(1, snippetDto)).ReturnsAsync(updatedSnippet);

            // Act
            var result = await _snippetsController.UpdateSnippet(1, snippetDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(updatedSnippet, okResult.Value);
        }

        [Fact]
        public async Task AddTagsToSnippet_ValidId_ReturnsUpdatedSnippet()
        {
            // Arrange
            var tagNames = new List<string> { "tag1", "tag2" };
            var updatedSnippet = MockDataFactory.CreateSnippetResponseDto();
            _snippetServiceMock.Setup(s => s.AddTagsToSnippetAsync(1, tagNames)).ReturnsAsync(updatedSnippet);

            // Act
            var result = await _snippetsController.AddTagsToSnippet(1, tagNames);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(updatedSnippet, okResult.Value);
        }

        [Fact]
        public async Task RemoveTagsFromSnippet_ValidId_ReturnsNoContent()
        {
            // Arrange
            var tagNames = new List<string> { "tag1", "tag2" };
            _snippetServiceMock.Setup(s => s.RemoveTagsFromSnippetAsync(1, tagNames)).Returns(Task.CompletedTask);

            // Act
            var result = await _snippetsController.RemoveTagsFromSnippet(1, tagNames);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task ReplaceTagsForSnippet_ValidId_ReturnsNoContent()
        {
            // Arrange
            var tagNames = new List<string> { "tag1", "tag2" };
            _snippetServiceMock.Setup(s => s.ReplaceTagsForSnippetAsync(1, tagNames)).Returns(Task.CompletedTask);

            // Act
            var result = await _snippetsController.ReplaceTagsForSnippet(1, tagNames);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task SoftDeleteSnippet_ValidId_ReturnsNoContent()
        {
            // Arrange
            _snippetServiceMock.Setup(s => s.SoftDeleteSnippetAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _snippetsController.SoftDeleteSnippet(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task SoftDeleteSnippet_InvalidId_ThrowsNotFoundException()
        {
            // Arrange
            _snippetServiceMock.Setup(s => s.SoftDeleteSnippetAsync(1)).ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _snippetsController.SoftDeleteSnippet(1));
        }

        [Fact]
        public async Task SearchSnippets_ReturnsOkWithSnippets()
        {
            // Arrange
            var snippets = new List<SnippetResponseDto> { MockDataFactory.CreateSnippetResponseDto() };
            _snippetServiceMock.Setup(s => s.SearchSnippetsAsync("keyword", "language", It.IsAny<List<string>>())).ReturnsAsync(snippets);

            // Act
            var result = await _snippetsController.SearchSnippets("keyword", "language", new List<string> { "tag1", "tag2" });

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(snippets, okResult.Value);
        }
    }
}
