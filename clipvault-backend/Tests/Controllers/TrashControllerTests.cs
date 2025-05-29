using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using ClipVault.Interfaces;
using ClipVault.Dtos;
using ClipVault.Exceptions;
using ClipVault.Tests.Mocks;
using Clipvault.Controllers;

namespace ClipVault.Tests.Controllers
{
    public class TrashControllerTests
    {
        private readonly Mock<ITrashService> _trashServiceMock;
        private readonly TrashController _controller;

        public TrashControllerTests()
        {
            _trashServiceMock = new Mock<ITrashService>();
            _controller = new TrashController(_trashServiceMock.Object);
        }

        [Fact]
        public async Task GetTrashedCollections_ReturnsOkWithCollections()
        {
            // Arrange
            var trashedCollections = MockDataFactory.CreateCollectionDtoList();
            _trashServiceMock.Setup(s => s.GetTrashedCollectionsAsync()).ReturnsAsync(trashedCollections);

            // Act
            var result = await _controller.GetTrashedCollections();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(trashedCollections, okResult.Value);
        }

        [Fact]
        public async Task RestoreCollection_ValidId_ReturnsOkWithCollection()
        {
            // Arrange
            var restored = MockDataFactory.CreateActiveCollectionDto();
            _trashServiceMock.Setup(s => s.RestoreCollectionAsync(2)).ReturnsAsync(restored);

            // Act
            var result = await _controller.RestoreCollection(2);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(restored, okResult.Value);
        }

        [Fact]
        public async Task RestoreCollection_InvalidId_ThrowsNotFoundException()
        {
            // Arrange
            _trashServiceMock.Setup(s => s.RestoreCollectionAsync(99)).ReturnsAsync(null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _controller.RestoreCollection(99));
        }

        [Fact]
        public async Task DeleteCollection_ValidId_ReturnsNoContent()
        {
            // Arrange
            _trashServiceMock.Setup(s => s.DeleteCollectionAsync(2)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteCollection(2);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteCollection_InvalidId_ThrowsNotFoundException()
        {
            // Arrange
            _trashServiceMock.Setup(s => s.DeleteCollectionAsync(99)).ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _controller.DeleteCollection(99));
        }

        [Fact]
        public async Task GetTrashedSnippets_ReturnsOkWithSnippets()
        {
            // Arrange
            var trashedSnippets = MockDataFactory.CreateSnippetResponseDtoList();
            _trashServiceMock.Setup(s => s.GetTrashedSnippetsAsync()).ReturnsAsync(trashedSnippets);

            // Act
            var result = await _controller.GetTrashedSnippets();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(trashedSnippets, okResult.Value);
        }

        [Fact]
        public async Task RestoreSnippet_ValidId_ReturnsOkWithSnippet()
        {
            // Arrange
            var restored = MockDataFactory.CreateSnippetResponseDto();
            _trashServiceMock.Setup(s => s.RestoreSnippetAsync(2)).ReturnsAsync(restored);

            // Act
            var result = await _controller.RestoreSnippet(2);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(restored, okResult.Value);
        }

        [Fact]
        public async Task RestoreSnippet_InvalidId_ThrowsNotFoundException()
        {
            // Arrange
            _trashServiceMock.Setup(s => s.RestoreSnippetAsync(99)).ReturnsAsync((SnippetResponseDto?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _controller.RestoreSnippet(99));
        }

        [Fact]
        public async Task DeleteSnippet_ValidId_ReturnsNoContent()
        {
            // Arrange
            _trashServiceMock.Setup(s => s.DeleteSnippetAsync(2)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteSnippet(2);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteSnippet_InvalidId_ThrowsNotFoundException()
        {
            // Arrange
            _trashServiceMock.Setup(s => s.DeleteSnippetAsync(99)).ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _controller.DeleteSnippet(99));
        }

        [Fact]
        public async Task EmptyTrash_ReturnsOkWithDeletedCount()
        {
            // Arrange
            _trashServiceMock.Setup(s => s.EmptyTrashAsync()).ReturnsAsync(5);

            // Act
            var result = await _controller.EmptyTrash();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            var deleted = okResult.Value.GetType().GetProperty("deleted")?.GetValue(okResult.Value);
            Assert.Equal(5, deleted);
        }
    }
}
