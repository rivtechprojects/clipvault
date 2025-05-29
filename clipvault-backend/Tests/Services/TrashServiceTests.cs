using ClipVault.Dtos;
using ClipVault.Interfaces;
using ClipVault.Models;
using ClipVault.Services;
using ClipVault.Tests.Mocks;
using Moq;
using Xunit;

namespace ClipVault.Tests.Services
{
    public class TrashServiceTests
    {
        private readonly Mock<IAppDbContext> _mockDbContext;
        private readonly Mock<ICollectionMapper> _mockCollectionMapper;
        private readonly Mock<ISnippetMapper> _mockSnippetMapper;
        private readonly TrashService _trashService;
        private List<Collection> _collections = new List<Collection>();
        private List<Snippet> _snippets = new List<Snippet>();
        private List<Language> _languages = new List<Language>();

        private void ResetTestData()
        {
            // Use MockDataFactory for consistent mock data
            _languages = MockDataFactory.CreateLanguages();
            _snippets = MockDataFactory.CreateSnippets(_languages);
            _collections = MockDataFactory.CreateCollections(_snippets);
        }

        private void SetupDbSets()
        {
            var mockCollectionSet = DbSetMockHelper.CreateMockDbSet(_collections.AsQueryable());
            var mockSnippetSet = DbSetMockHelper.CreateMockDbSet(_snippets.AsQueryable());
            var mockLanguageSet = DbSetMockHelper.CreateMockDbSet(_languages.AsQueryable());
            _mockDbContext.Setup(db => db.Collections).Returns(mockCollectionSet.Object);
            _mockDbContext.Setup(db => db.Snippets).Returns(mockSnippetSet.Object);
            _mockDbContext.Setup(db => db.Languages).Returns(mockLanguageSet.Object);
        }

        public TrashServiceTests()
        {
            _mockDbContext = new Mock<IAppDbContext>();
            _mockCollectionMapper = new Mock<ICollectionMapper>();
            _mockSnippetMapper = new Mock<ISnippetMapper>();
            ResetTestData();
            SetupDbSets();
            _mockDbContext.Setup(db => db.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            _mockCollectionMapper.Setup(m => m.MapToCollectionDto(It.IsAny<Collection>()))
                .Returns((Collection c) => new CollectionDto { Id = c.Id, Name = c.Name });
            _mockSnippetMapper.Setup(m => m.MapToSnippetResponseDto(It.IsAny<Snippet>()))
                .Returns((Snippet s) => new SnippetResponseDto { Id = s.Id, Title = s.Title, Code = s.Code, Language = s.Language?.Name ?? string.Empty });
            _trashService = new TrashService(_mockDbContext.Object, _mockCollectionMapper.Object, _mockSnippetMapper.Object);
        }

        [Fact]
        public async Task GetTrashedCollectionsAsync_ReturnsOnlyTrashed()
        {
            ResetTestData();
            SetupDbSets();
            var result = await _trashService.GetTrashedCollectionsAsync();
            Assert.Single(result);
            Assert.Equal("Trashed Collection", result[0].Name);
        }

        [Fact]
        public async Task GetTrashedSnippetsAsync_ReturnsOnlyTrashed()
        {
            ResetTestData();
            SetupDbSets();
            var result = await _trashService.GetTrashedSnippetsAsync();
            Assert.Single(result);
            Assert.Equal("Trashed Snippet", result[0].Title);
        }

        [Fact]
        public async Task RestoreCollectionAsync_RestoresCollection()
        {
            ResetTestData();
            SetupDbSets();
            var result = await _trashService.RestoreCollectionAsync(1);
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.False(_collections.First(c => c.Id == 1).IsDeleted);
        }

        [Fact]
        public async Task RestoreSnippetAsync_RestoresSnippet()
        {
            ResetTestData();
            SetupDbSets();
            var result = await _trashService.RestoreSnippetAsync(1);
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.False(_snippets.First(s => s.Id == 1).IsDeleted);
        }

        [Fact]
        public async Task DeleteCollectionAsync_DeletesCollection()
        {
            ResetTestData();
            SetupDbSets();
            var result = await _trashService.DeleteCollectionAsync(1);
            _collections.RemoveAll(c => c.Id == 1);
            SetupDbSets();
            Assert.True(result);
            Assert.DoesNotContain(_collections, c => c.Id == 1);
        }

        [Fact]
        public async Task DeleteSnippetAsync_DeletesSnippet()
        {
            ResetTestData();
            SetupDbSets();
            var result = await _trashService.DeleteSnippetAsync(1);
            _snippets.RemoveAll(s => s.Id == 1);
            SetupDbSets();
            Assert.True(result);
            Assert.DoesNotContain(_snippets, s => s.Id == 1);
        }

        [Fact]
        public async Task EmptyTrashAsync_DeletesAllTrashed()
        {
            ResetTestData();
            SetupDbSets();
            var deletedCount = await _trashService.EmptyTrashAsync();
            _collections.RemoveAll(c => c.IsDeleted);
            _snippets.RemoveAll(s => s.IsDeleted);
            SetupDbSets();
            Assert.True(deletedCount > 0);
            Assert.DoesNotContain(_collections, c => c.IsDeleted);
            Assert.DoesNotContain(_snippets, s => s.IsDeleted);
        }

        [Fact]
        public async Task GetTrashedCollectionsAsync_ShouldReturnTrashedCollections()
        {
            var result = await _trashService.GetTrashedCollectionsAsync();
            Assert.Single(result);
            Assert.Equal("Trashed Collection", result[0].Name);
        }

        [Fact]
        public async Task RestoreCollectionAsync_ShouldRestoreTrashedCollection()
        {
            var result = await _trashService.RestoreCollectionAsync(1);
            Assert.NotNull(result);
            Assert.False(_collections.First(c => c.Id == 1).IsDeleted);
        }

        [Fact]
        public async Task GetTrashedSnippetsAsync_ShouldReturnTrashedSnippets()
        {
            var result = await _trashService.GetTrashedSnippetsAsync();
            Assert.Single(result);
            Assert.Equal("Trashed Snippet", result[0].Title);
        }

        [Fact]
        public async Task RestoreSnippetAsync_ShouldRestoreTrashedSnippet()
        {
            var result = await _trashService.RestoreSnippetAsync(1);
            Assert.NotNull(result);
            Assert.False(_snippets.First(s => s.Id == 1).IsDeleted);
        }

        [Fact]
        public async Task DeleteSnippetAsync_ShouldDeleteTrashedSnippet()
        {
            ResetTestData();
            SetupDbSets();
            var deleted = await _trashService.DeleteSnippetAsync(1);
            // Remove the snippet from the list to reflect the deletion
            _snippets.RemoveAll(s => s.Id == 1);
            Assert.True(deleted);
            Assert.DoesNotContain(_snippets, s => s.Id == 1);
        }
    }
}
