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
    public class CollectionsControllerTests
    {
        private readonly Mock<ICollectionService> _collectionServiceMock;
        private readonly CollectionsController _controller;

        public CollectionsControllerTests()
        {
            _collectionServiceMock = new Mock<ICollectionService>();
            _controller = new CollectionsController(_collectionServiceMock.Object);
        }

        [Fact]
        public async Task CreateCollection_ValidRequest_ReturnsCreatedAtAction()
        {
            // Arrange
            var createDto = new CollectionCreateDto { Name = "Test Collection" };
            var collection = MockDataFactory.CreateActiveCollectionDto();
            _collectionServiceMock.Setup(s => s.CreateCollectionAsync(createDto)).ReturnsAsync(collection);

            // Act
            var result = await _controller.CreateCollection(createDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(_controller.GetCollectionById), createdAtActionResult.ActionName);
            Assert.Equal(collection, createdAtActionResult.Value);
        }

        [Fact]
        public async Task GetCollectionById_ValidId_ReturnsOkWithCollection()
        {
            // Arrange
            var collection = MockDataFactory.CreateActiveCollectionDto();
            _collectionServiceMock.Setup(s => s.GetCollectionWithSnippetsAsync(2)).ReturnsAsync(collection);

            // Act
            var result = await _controller.GetCollectionById(2);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(collection, okResult.Value);
        }

        [Fact]
        public async Task GetAllCollections_ReturnsOkWithCollections()
        {
            // Arrange
            var collections = MockDataFactory.CreateCollectionDtoList();
            _collectionServiceMock.Setup(s => s.GetAllCollectionsAsync()).ReturnsAsync(collections);

            // Act
            var result = await _controller.GetAllCollections();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(collections, okResult.Value);
        }

        [Fact]
        public async Task UpdateCollection_ValidId_ReturnsUpdatedCollection()
        {
            // Arrange
            var updateDto = new CollectionUpdateDto { Name = "Updated Collection" };
            var updatedCollection = MockDataFactory.CreateActiveCollectionDto();
            _collectionServiceMock.Setup(s => s.UpdateCollectionAsync(2, updateDto)).ReturnsAsync(updatedCollection);

            // Act
            var result = await _controller.UpdateCollection(2, updateDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(updatedCollection, okResult.Value);
        }

        [Fact]
        public async Task MoveCollection_ValidRequest_ReturnsOkWithUpdatedCollection()
        {
            // Arrange
            var updatedCollection = MockDataFactory.CreateActiveCollectionDto();
            _collectionServiceMock.Setup(s => s.MoveCollectionAsync(2, 1)).ReturnsAsync(updatedCollection);

            // Act
            var result = await _controller.MoveCollection(2, 1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(updatedCollection, okResult.Value);
        }

        [Fact]
        public async Task SoftDeleteCollection_ValidId_ReturnsNoContent()
        {
            // Arrange
            _collectionServiceMock.Setup(s => s.SoftDeleteCollectionAsync(2)).ReturnsAsync(true);

            // Act
            var result = await _controller.SoftDeleteCollection(2);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task SoftDeleteCollection_InvalidId_ThrowsNotFoundException()
        {
            // Arrange
            _collectionServiceMock.Setup(s => s.SoftDeleteCollectionAsync(99)).ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _controller.SoftDeleteCollection(99));
        }
    }
}
