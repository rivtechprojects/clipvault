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
        private readonly SnippetService _snippetService;

        public SnippetServiceTests()
        {
            _mockDbContext = new Mock<IAppDbContext>();

            var snippetDto = TestDataHelper.CreateSnippetCreateDto();
            var language = TestDataHelper.CreateLanguage();
            var tags = TestDataHelper.CreateTags();
            var snippet = TestDataHelper.CreateSnippet(snippetDto, language);

            snippet.SnippetTags = [.. tags.Select(tag => new SnippetTag { TagId = tag.Id, Tag = tag })];

            _mockTagService = ServiceMockHelper.CreateMockTagService(tags, snippetDto.TagNames);
            _mockSnippetMapper = ServiceMockHelper.CreateMockSnippetMapper(snippetDto, language, tags, snippet);

            // Enhance _snippetMapper mock setup
            _mockSnippetMapper.Setup(sm => sm.MapToSnippetResponseDto(It.IsAny<Snippet>()))
                .Returns((Snippet s) => new SnippetResponseDto
                {
                    Id = s.Id,
                    Title = s.Title,
                    Code = s.Code,
                    Language = s.Language?.Name ?? "C#", // Ensure non-null value
                    Tags = s.SnippetTags?.Select(st => st.Tag?.Name ?? string.Empty).ToList() ?? [] // Ensure non-null values
                });

            // Ensure _context.Snippets mock includes necessary data
            var snippetList = new List<Snippet>
            {
                snippet,
                TestDataHelper.CreateSnippet(new SnippetCreateDto { Title = "Another Snippet", Code = "Another Code", Language = "C#", TagNames = new List<string> { "test" } }, language)
            };
            var mockSnippetSetEnhanced = DbSetMockHelper.CreateMockDbSet(snippetList.AsQueryable());
            _mockDbContext.Setup(db => db.Snippets).Returns(mockSnippetSetEnhanced.Object);

            // Mock DbSet for Languages
            var languages = new List<Language> { language }.AsQueryable();
            var mockLanguageSet = DbSetMockHelper.CreateMockDbSet(languages);
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
            var snippet = TestDataHelper.CreateSnippet(snippetDto, language);

            // Mock DbSet for Languages
            var languages = new List<Language> { language }.AsQueryable();
            var mockLanguageSet = DbSetMockHelper.CreateMockDbSet(languages);
            _mockDbContext.Setup(db => db.Languages).Returns(mockLanguageSet.Object);

            // Mock DbSet for Snippets
            var snippets = new List<Snippet>().AsQueryable();
            var mockSnippetSet = DbSetMockHelper.CreateMockDbSet(snippets);
            _mockDbContext.Setup(db => db.Snippets).Returns(mockSnippetSet.Object);

            _mockTagService.Setup(ts => ts.ValidateAndCreateTagsAsync(snippetDto.TagNames)).ReturnsAsync(tags);
            _mockSnippetMapper.Setup(sm => sm.MapToSnippetEntity(snippetDto, language.Id, tags)).Returns(snippet);
            _mockSnippetMapper.Setup(sm => sm.MapToSnippetResponseDto(snippet)).Returns(new SnippetResponseDto
            {
                Id = snippet.Id,
                Title = snippet.Title,
                Code = snippet.Code,
                Language = language.Name,
                Tags = snippetDto.TagNames
            });

            // Act
            var result = await _snippetService.CreateSnippetAsync(snippetDto);

            // Assert
            AssertionHelper.AssertSnippetResponseDto(result, snippetDto, snippetDto.Language, snippetDto.TagNames);
        }

        [Fact]
        public async Task GetAllSnippetsAsync_ShouldReturnAllSnippets_WhenSnippetsExist()
        {
            // Arrange
            var language = TestDataHelper.CreateLanguage();
            var snippet1 = TestDataHelper.CreateSnippet(new SnippetCreateDto { Title = "Snippet 1", Code = "Code 1", Language = "C#", TagNames = new List<string> { "example" } }, language);
            var snippet2 = TestDataHelper.CreateSnippet(new SnippetCreateDto { Title = "Snippet 2", Code = "Code 2", Language = "C#", TagNames = new List<string> { "test" } }, language);

            var snippets = new List<Snippet> { snippet1, snippet2 }.AsQueryable();
            var mockSnippetSet = DbSetMockHelper.CreateMockDbSet(snippets);
            _mockDbContext.Setup(db => db.Snippets).Returns(mockSnippetSet.Object);

            // Act
            var result = await _snippetService.GetAllSnippetsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, r => r.Title == "Snippet 1" && r.Code == "Code 1");
            Assert.Contains(result, r => r.Title == "Snippet 2" && r.Code == "Code 2");
        }

        [Fact]
        public async Task GetSnippetByIdAsync_ShouldReturnSnippet_WhenSnippetExists()
        {
            // Arrange
            var language = TestDataHelper.CreateLanguage();
            var tags = TestDataHelper.CreateTags();
            var snippetWithLanguage = TestDataHelper.CreateSnippet(new SnippetCreateDto { Title = "Snippet 1", Code = "Code 1", Language = "C#", TagNames = new List<string> { "example" } }, language);
            snippetWithLanguage.Language = language; // Ensure Language is properly set

            var snippets = new List<Snippet> { snippetWithLanguage }.AsQueryable();
            var mockSnippetSet = DbSetMockHelper.CreateMockDbSet(snippets);
            _mockDbContext.Setup(db => db.Snippets).Returns(mockSnippetSet.Object);

            _mockSnippetMapper.Setup(sm => sm.MapToSnippetResponseDto(It.IsAny<Snippet>()))
                .Returns((Snippet s) => new SnippetResponseDto
                {
                    Id = s.Id,
                    Title = s.Title,
                    Code = s.Code,
                    Language = s.Language?.Name ?? "C#", // Ensure non-null value
                    Tags = s.SnippetTags?.Select(st => st.Tag?.Name ?? string.Empty).ToList() ?? new List<string>()
                });

            // Act
            var result = await _snippetService.GetSnippetByIdAsync(snippetWithLanguage.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(snippetWithLanguage.Title, result.Title);
            Assert.Equal(snippetWithLanguage.Code, result.Code);
            Assert.Equal(language.Name, result.Language);
        }

        [Fact]
        public async Task DeleteSnippetAsync_ShouldDeleteSnippet_WhenSnippetExists()
        {
            // Arrange
            var language = TestDataHelper.CreateLanguage();
            var snippetToDelete = TestDataHelper.CreateSnippet(new SnippetCreateDto { Title = "Snippet 1", Code = "Code 1", Language = "C#", TagNames = new List<string> { "example" } }, language);
            snippetToDelete.Id = 1; // Ensure ID matches the test case

            var snippetsForDelete = new List<Snippet> { snippetToDelete }.AsQueryable();
            var mockSnippetSetForDelete = DbSetMockHelper.CreateMockDbSet(snippetsForDelete);
            _mockDbContext.Setup(db => db.Snippets).Returns(mockSnippetSetForDelete.Object);
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
            var snippet = TestDataHelper.CreateSnippet(new SnippetCreateDto { Title = "Old Title", Code = "Old Code", Language = "C#", TagNames = new List<string> { "example" } }, TestDataHelper.CreateLanguage());

            var snippets = new List<Snippet> { snippet }.AsQueryable();
            var mockSnippetSet = DbSetMockHelper.CreateMockDbSet(snippets);
            _mockDbContext.Setup(db => db.Snippets).Returns(mockSnippetSet.Object);
            _mockDbContext.Setup(db => db.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var updateDto = new SnippetUpdateDto { Title = "Updated Title", Code = "Updated Code" };

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
            // Arrange
            var language = TestDataHelper.CreateLanguage();
            language.Name = "C#";

            var snippetWithTags = TestDataHelper.CreateSnippet(new SnippetCreateDto { Title = "Keyword Match", Code = "Code 1", Language = "C#", TagNames = new List<string> { "example" } }, language);
            snippetWithTags.Language = language;
            snippetWithTags.SnippetTags = [new SnippetTag { Tag = new Tag { Name = "example" } }]; // Ensure SnippetTags and Tag are initialized

            var snippet2 = TestDataHelper.CreateSnippet(new SnippetCreateDto { Title = "No Match", Code = "Code 2", Language = "C#", TagNames = new List<string> { "test" } }, language);
            snippet2.Language = language;
            snippet2.SnippetTags = new List<SnippetTag> { new() { Tag = new Tag { Name = "test" } } };

            var snippets = new List<Snippet> { snippetWithTags, snippet2 }.AsQueryable();
            var mockSnippetSet = DbSetMockHelper.CreateMockDbSet(snippets);
            _mockDbContext.Setup(db => db.Snippets).Returns(mockSnippetSet.Object);

            // Normalize mock data
            snippetWithTags.Title = snippetWithTags.Title.ToLower();
            snippetWithTags.Code = snippetWithTags.Code.ToLower();
            snippetWithTags.SnippetTags.ForEach(st => {
                if (st.Tag?.Name != null)
                {
                    st.Tag.Name = st.Tag.Name.ToLower();
                }
            });

            snippet2.Title = snippet2.Title.ToLower();
            snippet2.Code = snippet2.Code.ToLower();
            snippet2.SnippetTags.ForEach(st => {
                if (st.Tag?.Name != null)
                {
                    st.Tag.Name = st.Tag.Name.ToLower();
                }
            });

            // Act
            var result = await _snippetService.SearchSnippetsAsync("keyword", "c#", ["example"]);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("keyword match", result[0].Title);
        }

        [Fact]
        public async Task ReplaceTagsForSnippetAsync_ShouldReplaceTags_WhenSnippetExists()
        {
            // Arrange
            var language = TestDataHelper.CreateLanguage();
            var snippetForReplacingTags = TestDataHelper.CreateSnippet(new SnippetCreateDto { Title = "Snippet 1", Code = "Code 1", Language = "C#", TagNames = ["oldTag"] }, language);
            snippetForReplacingTags.SnippetTags = [new() { Tag = new Tag { Name = "oldTag" } }];

            var snippets = new List<Snippet> { snippetForReplacingTags }.AsQueryable();
            var mockSnippetSet = DbSetMockHelper.CreateMockDbSet(snippets);
            _mockDbContext.Setup(db => db.Snippets).Returns(mockSnippetSet.Object);
            _mockDbContext.Setup(db => db.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var newTags = new List<string> { "newTag" };
            _mockTagService.Setup(ts => ts.ValidateAndCreateTagsAsync(It.IsAny<List<string>>()))
                .ReturnsAsync([new() { Id = 2, Name = "newTag" }]);

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
            var language = TestDataHelper.CreateLanguage();
            var snippetForAddingTags = TestDataHelper.CreateSnippet(new SnippetCreateDto { Title = "Snippet 1", Code = "Code 1", Language = "C#", TagNames = new List<string> { "example" } }, language);
            snippetForAddingTags.SnippetTags = [];

            var snippets = new List<Snippet> { snippetForAddingTags }.AsQueryable();
            var mockSnippetSet = DbSetMockHelper.CreateMockDbSet(snippets);
            _mockDbContext.Setup(db => db.Snippets).Returns(mockSnippetSet.Object);
            _mockDbContext.Setup(db => db.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            _mockTagService.Setup(ts => ts.ValidateAndCreateTagsAsync(It.IsAny<List<string>>()))
                .ReturnsAsync([new Tag { Id = 2, Name = "newTag" }]);

            var newTags = new List<string> { "newTag" };

            // Act
            await _snippetService.AddTagsToSnippetAsync(snippetForAddingTags.Id, newTags);

            // Assert
            _mockDbContext.Verify(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.Single(snippetForAddingTags.SnippetTags);
            Assert.Equal(2, snippetForAddingTags.SnippetTags[0].TagId);
        }

        [Fact]
        public async Task RemoveTagsFromSnippetAsync_ShouldRemoveTags_WhenSnippetExists()
        {
            // Arrange
            var snippet = TestDataHelper.CreateSnippet(new SnippetCreateDto { Title = "Snippet 1", Code = "Code 1", Language = "C#", TagNames = new List<string> { "example" } }, TestDataHelper.CreateLanguage());
            snippet.SnippetTags = [new() { TagId = 1, Tag = new Tag { Id = 1, Name = "example" } }];

            var snippets = new List<Snippet> { snippet }.AsQueryable();
            var mockSnippetSet = DbSetMockHelper.CreateMockDbSet(snippets);
            _mockDbContext.Setup(db => db.Snippets).Returns(mockSnippetSet.Object);
            _mockDbContext.Setup(db => db.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var tagsToRemove = new List<string> { "example" };

            // Act
            await _snippetService.RemoveTagsFromSnippetAsync(snippet.Id, tagsToRemove);

            // Assert
            _mockDbContext.Verify(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.Empty(snippet.SnippetTags);
        }
    }
}
