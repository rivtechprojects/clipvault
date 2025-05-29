using ClipVault.Dtos;
using ClipVault.Interfaces;
using ClipVault.Models;
using ClipVault.Services;
using ClipVault.Tests.Mocks;
using Moq;
using Xunit;

namespace ClipVault.Tests.Services
{
    public class CollectionServiceTests
    {
        private readonly Mock<IAppDbContext> _mockDbContext;
        private readonly Mock<ICollectionMapper> _mockCollectionMapper;
        private readonly ICollectionService _collectionService;
        private readonly List<Collection> _collections;
        private readonly List<Snippet> _snippets;
        private readonly List<Language> _languages;

        public CollectionServiceTests()
        {
            _mockDbContext = new Mock<IAppDbContext>();
            _mockCollectionMapper = new Mock<ICollectionMapper>();

            // Use MockDataFactory for consistent mock data
            _languages = MockDataFactory.CreateLanguages();
            _snippets = MockDataFactory.CreateSnippets(_languages);
            _collections = MockDataFactory.CreateCollections(_snippets);

            var mockCollectionSet = DbSetMockHelper.CreateMockDbSet(_collections.AsQueryable());
            var mockSnippetSet = DbSetMockHelper.CreateMockDbSet(_snippets.AsQueryable());
            _mockDbContext.Setup(db => db.Collections).Returns(mockCollectionSet.Object);
            _mockDbContext.Setup(db => db.Snippets).Returns(mockSnippetSet.Object);
            _mockDbContext.Setup(db => db.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            _mockCollectionMapper.Setup(m => m.MapToCollectionDto(It.IsAny<Collection>()))
                .Returns((Collection c) => new CollectionDto { Id = c.Id, Name = c.Name, ParentCollectionId = c.ParentCollectionId, Snippets = c.Snippets.Select(s => new SnippetResponseDto { Id = s.Id, Title = s.Title, Code = s.Code, Language = s.Language.Name, Tags = new List<string>(), CollectionId = c.Id }).ToList(), SubCollections = new List<CollectionDto>() });

            _collectionService = new CollectionService(_mockDbContext.Object, _mockCollectionMapper.Object);
        }

        [Fact]
        public async Task GetAllCollectionsAsync_ShouldReturnAllCollections()
        {
            var result = await _collectionService.GetAllCollectionsAsync();
            Assert.NotNull(result);
            Assert.Single(result); // Only one active collection
            Assert.Equal("Active Collection", result[0].Name);
        }

        [Fact]
        public async Task GetCollectionWithSnippetsAsync_ShouldReturnCollectionWithSnippets()
        {
            var result = await _collectionService.GetCollectionWithSnippetsAsync(2); // Active Collection has Id = 2
            Assert.NotNull(result);
            Assert.Equal(2, result.Id);
            Assert.NotNull(result.Snippets);
            Assert.Single(result.Snippets);
            Assert.Equal("Active Snippet", result.Snippets[0].Title);
        }

        [Fact]
        public async Task CreateCollectionAsync_ShouldCreateCollection()
        {
            var dto = new CollectionCreateDto { Name = "New Collection", ParentCollectionId = null };
            _mockCollectionMapper.Setup(m => m.MapToCollectionEntity(dto)).Returns(new Collection { Id = 3, Name = dto.Name, ParentCollectionId = dto.ParentCollectionId });
            _mockCollectionMapper.Setup(m => m.MapToCollectionDto(It.IsAny<Collection>())).Returns((Collection c) => new CollectionDto { Id = c.Id, Name = c.Name, ParentCollectionId = c.ParentCollectionId });

            var result = await _collectionService.CreateCollectionAsync(dto);
            Assert.NotNull(result);
            Assert.Equal("New Collection", result.Name);
        }

        [Fact]
        public async Task UpdateCollectionAsync_ShouldUpdateCollectionName()
        {
            var updateDto = new CollectionUpdateDto { Name = "Updated Name" };
            var result = await _collectionService.UpdateCollectionAsync(2, updateDto); // Active Collection has Id = 2
            Assert.NotNull(result);
            Assert.Equal("Updated Name", result.Name);
        }

        [Fact]
        public async Task SoftDeleteCollectionAsync_ShouldSoftDeleteCollection()
        {
            var result = await _collectionService.SoftDeleteCollectionAsync(2); // Active Collection has Id = 2
            Assert.True(result);
            Assert.True(_collections.First(c => c.Id == 2).IsDeleted);
        }

        [Fact]
        public async Task MoveCollectionAsync_ShouldMoveCollectionToNewParent()
        {
            // Add a new parent collection
            var parent = new Collection { Id = 10, Name = "Parent", ParentCollectionId = null, IsDeleted = false, Snippets = new List<Snippet>(), SubCollections = new List<Collection>() };
            _collections.Add(parent);
            var mockCollectionSet = DbSetMockHelper.CreateMockDbSet(_collections.AsQueryable());
            _mockDbContext.Setup(db => db.Collections).Returns(mockCollectionSet.Object);

            var result = await _collectionService.MoveCollectionAsync(1, 10);
            Assert.NotNull(result);
            Assert.Equal(10, _collections.First(c => c.Id == 1).ParentCollectionId);
        }
    }
}
