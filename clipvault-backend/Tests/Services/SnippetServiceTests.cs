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

            var snippetDto = TestDataHelper.CreateSnippetCreateDto();
            var language = TestDataHelper.CreateLanguage();
            var tags = TestDataHelper.CreateTags();
            var snippetList = TestDataHelper.CreateSnippetList();

            _mockTagService = ServiceMockHelper.CreateMockTagService(tags, snippetDto.TagNames);
            _mockSnippetMapper = ServiceMockHelper.CreateMockSnippetMapper(snippetDto, language, tags, snippetList.First());

            _mockSnippetMapper.Setup(sm => sm.MapToSnippetResponseDto(It.IsAny<Snippet>()))
                .Returns((Snippet s) => TestDataHelper.CreateSnippetResponseDto(s));

            _mockSnippetMapper.Setup(sm => sm.MapToSnippetEntity(It.IsAny<SnippetCreateDto>(), It.IsAny<int>(), It.IsAny<List<Tag>>()))
                .Returns((SnippetCreateDto dto, int langId, List<Tag> tagList) =>
                    TestDataHelper.CreateSnippet(dto, langId, tagList));

            // Mock DbSet for Snippets
            var mockSnippetSet = DbSetMockHelper.CreateMockDbSet(snippetList.AsQueryable());
            _mockDbContext.Setup(db => db.Snippets).Returns(mockSnippetSet.Object);

            // Mock DbSet for Languages
            var mockLanguageSet = DbSetMockHelper.CreateMockDbSet(new List<Language> { language }.AsQueryable());
            _mockDbContext.Setup(db => db.Languages).Returns(mockLanguageSet.Object);

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
            var snippetDto = TestDataHelper.CreateSnippetCreateDto();
            var language = TestDataHelper.CreateLanguage();
            var tags = TestDataHelper.CreateTags();
            var snippet = TestDataHelper.CreateSnippet(snippetDto, language.Id, tags);

            _mockTagService.Setup(ts => ts.ValidateAndCreateTagsAsync(snippetDto.TagNames))
                .ReturnsAsync(snippet.SnippetTags.Select(st => st.Tag!).ToList());

            _mockSnippetMapper.Setup(sm => sm.MapToSnippetEntity(snippetDto, It.IsAny<int>(), It.IsAny<List<Tag>>()))
                .Returns(snippet);

            _mockSnippetMapper.Setup(sm => sm.MapToSnippetResponseDto(It.IsAny<Snippet>()))
                .Returns(new SnippetResponseDto
                {
                    Id = snippet.Id,
                    Title = snippet.Title,
                    Code = snippet.Code,
                    Language = snippet.Language?.Name ?? "",
                    Tags = snippet.SnippetTags.Select(st => st.Tag?.Name ?? "").ToList()
                });

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
            Assert.Equal(3, result.Count);
            Assert.Contains(result, r => r.Title == "Snippet 1" && r.Code == "Code 1");
            Assert.Contains(result, r => r.Title == "Snippet 2" && r.Code == "Code 2");
            Assert.Contains(result, r => r.Title == "Snippet 3" && r.Code == "Code 3");
        }

        [Fact]
        public async Task GetSnippetByIdAsync_ShouldReturnSnippet_WhenSnippetExists()
        {
            // Act
            var result = await _snippetService.GetSnippetByIdAsync(1);
            // Assert
            Assert.NotNull(result);
            Assert.Equal("Snippet 1", result.Title);
            Assert.Equal("Code 1", result.Code);
            Assert.Equal("Python", result.Language);
        }

        [Fact]
        public async Task DeleteSnippetAsync_ShouldDeleteSnippet_WhenSnippetExists()
        {
            // Arrange
            var snippetToDelete = TestDataHelper.CreateSnippetList().First(); // Use the first snippet from the list

            _mockDbContext.Setup(db => db.Snippets.Remove(It.IsAny<Snippet>()));
            _mockDbContext.Setup(db => db.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _snippetService.DeleteSnippetAsync(snippetToDelete.Id);

            // Assert
            Assert.True(result);
            _mockDbContext.Verify(db => db.Snippets.Remove(It.Is<Snippet>(s => s.Id == snippetToDelete.Id)), Times.Once);
            _mockDbContext.Verify(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateSnippetAsync_ShouldUpdateSnippet_WhenSnippetExists()
        {
            // Arrange
            var snippet = TestDataHelper.CreateSnippetList().First(); // Use the first snippet from the list
            var updateDto = new SnippetUpdateDto { Title = "Updated Title", Code = "Updated Code" };

            _mockDbContext.Setup(db => db.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

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
            // Scenario 1: Single matching tag
            var result = await _snippetService.SearchSnippetsAsync("Snippet", "C#", new List<string> { "example" });
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Snippet 2", result[0].Title);

            // Scenario 2: Multiple matching tags
            result = await _snippetService.SearchSnippetsAsync("Code", "Python", new List<string> { "automation", "scripting" });
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Snippet 1", result[0].Title);

            // Scenario 3: No matching tags
            result = await _snippetService.SearchSnippetsAsync("nonexistent", "C#", new List<string> { "nonexistent" });
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task ReplaceTagsForSnippetAsync_ShouldReplaceTags_WhenSnippetExists()
        {
            // Arrange
            var snippets = TestDataHelper.CreateSnippetList().AsQueryable();
            var snippetForReplacingTags = snippets.First(); // Use the first snippet from the list

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
            var snippets = TestDataHelper.CreateSnippetList();
            var newTags = new List<Tag> { new Tag { Id = 5, Name = "newTag" } };
            TestDataHelper.AddTagsToSnippet(snippets, 3, newTags);

            var mockSnippetSet = DbSetMockHelper.CreateMockDbSet(snippets.AsQueryable());
            _mockDbContext.Setup(db => db.Snippets).Returns(mockSnippetSet.Object);
            _mockDbContext.Setup(db => db.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            _mockTagService.Setup(ts => ts.ValidateAndCreateTagsAsync(It.IsAny<List<string>>()))
                .ReturnsAsync(newTags);

            // Act
            var result = await _snippetService.AddTagsToSnippetAsync(3, new List<string> { "newTag" });

            // Assert
            _mockDbContext.Verify(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.Single(snippets.First(s => s.Id == 3).SnippetTags);
            Assert.Contains("newTag", result.Tags);
        }

        [Fact]
        public async Task RemoveTagsFromSnippetAsync_ShouldRemoveTags_WhenSnippetExists()
        {
            // Arrange
            var snippets = TestDataHelper.CreateSnippetList().AsQueryable();
            var snippet = snippets.First(); // Use the first snippet from the list

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
            var snippets = TestDataHelper.CreateSnippetList().AsQueryable();
            var mockSnippetSet = DbSetMockHelper.CreateMockDbSet(snippets);
            _mockDbContext.Setup(db => db.Snippets).Returns(mockSnippetSet.Object);

            // Act
            var result = await _snippetService.GetSnippetsDtoByTagAsync(tagName);

            // Assert
            Assert.NotNull(result);
            if (result.Any())
            {
                Assert.All(result, snippet => Assert.Contains(tagName, snippet.Tags)); // Validate all returned snippets
            }
            else
            {
                Assert.Empty(result); // Ensure the result is empty if no snippets match
            }
        }
    }
}
