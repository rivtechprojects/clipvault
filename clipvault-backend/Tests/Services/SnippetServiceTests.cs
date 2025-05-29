using ClipVault.Dtos;
using ClipVault.Interfaces;
using ClipVault.Models;
using ClipVault.Services;
using Moq;
using Xunit;
using ClipVault.Tests.Mocks;

namespace ClipVault.Tests
{
    public class SnippetServiceTests
    {
        private readonly Mock<IAppDbContext> _mockDbContext;
        private readonly Mock<ITagService> _mockTagService;
        private readonly Mock<ISnippetMapper> _mockSnippetMapper;
        private readonly ISnippetService _snippetService;

        public SnippetServiceTests()
        {
            _mockDbContext = new Mock<IAppDbContext>();

            var languages = MockDataFactory.CreateLanguages();
            var snippets = MockDataFactory.CreateSnippets(languages);
            var tags = MockDataFactory.CreateTags();

            var snippetDto = MockDataFactory.CreateSnippetCreateDto();

            _mockTagService = ServiceMockHelper.CreateMockTagService(tags, snippetDto.TagNames);
            _mockSnippetMapper = ServiceMockHelper.CreateMockSnippetMapper(snippetDto, languages.First(), tags, snippets.First());

            _mockSnippetMapper.Setup(sm => sm.MapToSnippetResponseDto(It.IsAny<Snippet>()))
                .Returns((Snippet s) => MockDataFactory.CreateSnippetResponseDto());

            _mockSnippetMapper.Setup(sm => sm.MapToSnippetEntity(It.IsAny<SnippetCreateDto>(), It.IsAny<int>(), It.IsAny<List<Tag>>()))
                .Returns((SnippetCreateDto dto, int langId, List<Tag> tagList) => MockDataFactory.CreateActiveSnippetWithTags());

            // Mock DbSet for Snippets
            var mockSnippetSet = DbSetMockHelper.CreateMockDbSet(snippets.AsQueryable());
            _mockDbContext.Setup(db => db.Snippets).Returns(mockSnippetSet.Object);

            // Mock DbSet for Languages
            var mockLanguageSet = DbSetMockHelper.CreateMockDbSet(languages.AsQueryable());
            _mockDbContext.Setup(db => db.Languages).Returns(mockLanguageSet.Object);

            // Mock DbSet for Collections
            var mockCollectionSet = DbSetMockHelper.CreateMockDbSet(new List<Collection>().AsQueryable());
            _mockDbContext.Setup(db => db.Collections).Returns(mockCollectionSet.Object);

            _snippetService = new SnippetService(
                _mockDbContext.Object,
                _mockTagService.Object,
                _mockSnippetMapper.Object
            );
        }

        [Fact]
        public async Task CreateSnippetAsync_ShouldCreateSnippet_WhenDataIsValid()
        {
            // Arrange
            var snippetDto = MockDataFactory.CreateSnippetCreateDto();
            var snippet = MockDataFactory.CreateActiveSnippetWithTags();

            _mockTagService.Setup(ts => ts.ValidateAndCreateTagsAsync(snippetDto.TagNames))
                .ReturnsAsync(snippet.SnippetTags.Select(st => st.Tag!).ToList());

            _mockSnippetMapper.Setup(sm => sm.MapToSnippetEntity(snippetDto, It.IsAny<int>(), It.IsAny<List<Tag>>()))
                .Returns(snippet);

            _mockSnippetMapper.Setup(sm => sm.MapToSnippetResponseDto(It.IsAny<Snippet>()))
                .Returns(MockDataFactory.CreateSnippetResponseDto());

            // Act
            var result = await _snippetService.CreateSnippetAsync(snippetDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(snippetDto.Title, result.Title);
            Assert.Equal(snippetDto.Code, result.Code);
            Assert.Equal(snippetDto.Language, result.Language);
            Assert.Equal(snippetDto.TagNames, result.Tags);
        }

        [Fact]
        public async Task GetAllSnippetsAsync_ShouldReturnAllSnippets_WhenSnippetsExist()
        {
            // Act
            var result = await _snippetService.GetAllSnippetsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result); // Only one active snippet in MockDataFactory
            Assert.Equal("Active Snippet", result[0].Title);
            Assert.Equal("Sample code 2", result[0].Code);
        }

        [Fact]
        public async Task GetSnippetByIdAsync_ShouldReturnSnippet_WhenSnippetExists()
        {
            // Act
            var result = await _snippetService.GetSnippetByIdAsync(2); // Active Snippet has Id = 2
            // Assert
            Assert.NotNull(result);
            Assert.Equal("Active Snippet", result.Title);
            Assert.Equal("Sample code 2", result.Code);
            Assert.Equal("Python", result.Language); // Language for Id=2 in MockDataFactory
        }

        [Fact]
        public async Task SoftDeleteSnippetAsync_ShouldSoftDeleteSnippet_WhenSnippetExists()
        {
            // Arrange
            var snippets = MockDataFactory.CreateSnippets(MockDataFactory.CreateLanguages());
            var snippetToDelete = snippets.First(s => !s.IsDeleted);
            snippetToDelete.IsDeleted = false;
            var mockSnippetSet = DbSetMockHelper.CreateMockDbSet(new List<Snippet> { snippetToDelete }.AsQueryable());
            _mockDbContext.Setup(db => db.Snippets).Returns(mockSnippetSet.Object);
            _mockDbContext.Setup(db => db.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _snippetService.SoftDeleteSnippetAsync(snippetToDelete.Id);

            // Assert
            Assert.True(result);
            Assert.True(snippetToDelete.IsDeleted);
            _mockDbContext.Verify(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateSnippetAsync_ShouldUpdateSnippet_WhenSnippetExists()
        {
            // Arrange
            var snippets = MockDataFactory.CreateSnippets(MockDataFactory.CreateLanguages());
            var snippet = snippets.First(s => !s.IsDeleted); // Use the active snippet
            var updateDto = MockDataFactory.CreateSnippetUpdateDto();
            var mockSnippetSet = DbSetMockHelper.CreateMockDbSet(snippets.AsQueryable());
            _mockDbContext.Setup(db => db.Snippets).Returns(mockSnippetSet.Object);
            _mockDbContext.Setup(db => db.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Setup the mapper to return the updated values
            _mockSnippetMapper.Setup(sm => sm.MapToSnippetResponseDto(It.IsAny<Snippet>()))
                .Returns(new SnippetResponseDto
                {
                    Id = snippet.Id,
                    Title = updateDto.Title ?? string.Empty,
                    Code = updateDto.Code ?? string.Empty,
                    Language = snippet.Language?.Name ?? string.Empty,
                    Tags = snippet.SnippetTags.Select(st => st.Tag?.Name ?? string.Empty).ToList()
                });

            // Act
            var result = await _snippetService.UpdateSnippetAsync(snippet.Id, updateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(updateDto.Title, result.Title);
            Assert.Equal(updateDto.Code, result.Code);
            _mockDbContext.Verify(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SearchSnippetsAsync_ShouldReturnMatchingSnippets_WhenCriteriaMatch()
        {
            // Act & Assert
            // Scenario 1: Title and language match active snippet
            var result = await _snippetService.SearchSnippetsAsync("Active", "Python", new List<string>());
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Active Snippet", result[0].Title);

            // Scenario 2: No matching tags
            result = await _snippetService.SearchSnippetsAsync("nonexistent", "C#", new List<string> { "nonexistent" });
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task ReplaceTagsForSnippetAsync_ShouldReplaceTags_WhenSnippetExists()
        {
            // Arrange
            var snippets = MockDataFactory.CreateSnippets(MockDataFactory.CreateLanguages()).AsQueryable();
            var snippetForReplacingTags = snippets.First(s => s.Id == 2); // Use the active snippet

            var mockSnippetSet = DbSetMockHelper.CreateMockDbSet(snippets);
            _mockDbContext.Setup(db => db.Snippets).Returns(mockSnippetSet.Object);
            _mockDbContext.Setup(db => db.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var newTags = new List<string> { "newTag" };
            _mockTagService.Setup(ts => ts.ValidateAndCreateTagsAsync(It.IsAny<List<string>>()))
                .ReturnsAsync(new List<Tag> { new Tag { Id = 2, Name = "newTag" } });

            // Act
            await _snippetService.ReplaceTagsForSnippetAsync(snippetForReplacingTags.Id, newTags);

            // Assert
            _mockDbContext.Verify(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.Single(snippetForReplacingTags.SnippetTags);
            Assert.Equal(2, snippetForReplacingTags.SnippetTags[0].TagId);
        }

        [Fact]
        public async Task AddTagsToSnippetAsync_ShouldAddTags_WhenSnippetExists()
        {
            // Arrange
            var snippets = MockDataFactory.CreateSnippets(MockDataFactory.CreateLanguages());
            var newTags = new List<Tag> { new Tag { Id = 5, Name = "newTag" } };
            var activeSnippet = snippets.First(s => s.Id == 2);
            activeSnippet.SnippetTags.AddRange(newTags.Select(t => new SnippetTag { Tag = t, TagId = t.Id }));

            var mockSnippetSet = DbSetMockHelper.CreateMockDbSet(snippets.AsQueryable());
            _mockDbContext.Setup(db => db.Snippets).Returns(mockSnippetSet.Object);
            _mockDbContext.Setup(db => db.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            _mockTagService.Setup(ts => ts.ValidateAndCreateTagsAsync(It.IsAny<List<string>>()))
                .ReturnsAsync(newTags);

            // Setup the mapper to return the updated tags
            _mockSnippetMapper.Setup(sm => sm.MapToSnippetResponseDto(It.IsAny<Snippet>()))
                .Returns(new SnippetResponseDto
                {
                    Id = activeSnippet.Id,
                    Title = activeSnippet.Title,
                    Code = activeSnippet.Code,
                    Language = activeSnippet.Language?.Name ?? string.Empty,
                    Tags = activeSnippet.SnippetTags.Select(st => st.Tag?.Name ?? string.Empty).ToList()
                });

            // Act
            var result = await _snippetService.AddTagsToSnippetAsync(2, new List<string> { "newTag" });

            // Assert
            Assert.NotNull(result);
            Assert.Contains("newTag", result.Tags);
            _mockDbContext.Verify(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.Contains(activeSnippet.SnippetTags, st => st.TagId == 5);
        }

        [Fact]
        public async Task RemoveTagsFromSnippetAsync_ShouldRemoveTags_WhenSnippetExists()
        {
            // Arrange
            var snippets = MockDataFactory.CreateSnippets(MockDataFactory.CreateLanguages()).AsQueryable();
            var snippet = snippets.First(s => s.Id == 2); // Use the active snippet

            var mockSnippetSet = DbSetMockHelper.CreateMockDbSet(snippets);
            _mockDbContext.Setup(db => db.Snippets).Returns(mockSnippetSet.Object);
            _mockDbContext.Setup(db => db.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var tagsToRemove = new List<string> { "example" };

            // Act
            await _snippetService.RemoveTagsFromSnippetAsync(snippet.Id, tagsToRemove);

            // Assert
            _mockDbContext.Verify(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.NotNull(snippet.SnippetTags);
            Assert.DoesNotContain(snippet.SnippetTags, st => st.Tag?.Name == "example");
        }

        [Fact]
        public async Task GetSnippetsDtoByTagAsync_ShouldReturnSnippets_WhenTagExists()
        {
            // Arrange
            var tagName = "example";
            var snippets = MockDataFactory.CreateSnippets(MockDataFactory.CreateLanguages()).AsQueryable();
            var mockSnippetSet = DbSetMockHelper.CreateMockDbSet(snippets);
            _mockDbContext.Setup(db => db.Snippets).Returns(mockSnippetSet.Object);

            // Act
            var result = await _snippetService.GetSnippetsDtoByTagAsync(tagName);

            // Assert
            Assert.NotNull(result);
            if (result.Any())
            {
                Assert.All(result, snippet => Assert.Contains(tagName, snippet.Tags));
            }
            else
            {
                Assert.Empty(result);
            }
        }
    }
}
