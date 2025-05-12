using ClipVault.Interfaces;
using ClipVault.Tests.Mocks;
using Moq;
using Xunit;
using ClipVault.Dtos;
using ClipVault.Models;

namespace ClipVault.Tests.Mappers
{
    public class SnippetMapperTests
    {
        private readonly Mock<ISnippetMapper> _mapperMock;
        private readonly ISnippetMapper _mapper;

        public SnippetMapperTests()
        {
            _mapperMock = new Mock<ISnippetMapper>();
            _mapper = _mapperMock.Object;

            // Mock setup for MapToSnippetResponseDto
            _mapperMock.Setup(m => m.MapToSnippetResponseDto(It.IsAny<Snippet>()))
                .Returns((Snippet snippet) => TestDataHelper.CreateSnippetResponseDto(snippet));

            _mapperMock.Setup(m => m.MapToSnippetEntity(It.IsAny<SnippetCreateDto>(), It.IsAny<int>(), It.IsAny<List<Tag>>()))
            .Returns((SnippetCreateDto dto, int languageId, List<Tag> tags) =>
                TestDataHelper.CreateSnippet(dto, languageId, tags));
            // Mock setup for MapToUpdateDto
            _mapperMock.Setup(m => m.MapToUpdateDto(It.IsAny<SnippetResponseDto>()))
                .Returns((SnippetResponseDto response) => TestDataHelper.CreateSnippetUpdateDto(response));
        }

        [Fact]
        public void MapToSnippetResponseDto_ShouldMapCorrectly()
        {
            // Arrange
            var snippetDto = TestDataHelper.CreateSnippetCreateDto();
            var language = TestDataHelper.CreateLanguage();
            var tags = TestDataHelper.CreateTags();
            var snippet = TestDataHelper.CreateSnippet(snippetDto, language.Id, tags);
            snippet.SnippetTags = new List<SnippetTag>
            {
                new SnippetTag { Tag = new Tag { Name = "Tag1" } },
                new SnippetTag { Tag = new Tag { Name = "Tag2" } }
            };

            // Act
            var result = _mapper.MapToSnippetResponseDto(snippet);

            var expectedTags = snippet.SnippetTags?
                .Where(st => st != null && st.Tag != null)
                .Select(st => st.Tag!.Name)
                .ToList() ?? new List<string>();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(snippet.Id, result.Id);
            Assert.Equal(snippet.Title, result.Title);
            Assert.Equal(snippet.Code, result.Code);
            Assert.Equal(snippet.Language?.Name ?? string.Empty, result.Language);
            Assert.NotNull(result.Tags);
            Assert.Equal(expectedTags, result.Tags);
        }

        [Fact]
        public void MapToSnippetEntity_ShouldMapCorrectly()
        {
            // Arrange
            var snippetDto = TestDataHelper.CreateSnippetCreateDto();
            var tags = TestDataHelper.CreateTags();
            int languageId = 1;

            // Act
            var result = _mapper.MapToSnippetEntity(snippetDto, languageId, tags);

            // Assert
            Assert.Equal(snippetDto.Title, result.Title);
            Assert.Equal(snippetDto.Code, result.Code);
            Assert.Equal(languageId, result.LanguageId);
            Assert.NotNull(result.SnippetTags);
            Assert.All(result.SnippetTags, st => Assert.NotNull(st.Tag));
            Assert.All(result.SnippetTags, st => Assert.NotNull(st.Tag?.Name));
            Assert.Equal(tags.Select(t => t.Id).ToList(), result.SnippetTags.Select(st => st.TagId).ToList());
        }

        [Fact]
        public void MapToUpdateDto_ShouldMapCorrectly()
        {
            // Arrange
            var snippetResponse = TestDataHelper.CreateSnippetResponseDto();

            // Act
            var result = _mapper.MapToUpdateDto(snippetResponse);

            // Assert
            Assert.Equal(snippetResponse.Title, result.Title);
            Assert.Equal(snippetResponse.Code, result.Code);
            Assert.Equal(snippetResponse.Language ?? string.Empty, result.Language);
            Assert.Equal(snippetResponse.Tags ?? new List<string>(), result.TagNames);
        }
    }
}
