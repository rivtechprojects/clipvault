using ClipVault.Tests.Mocks;
using Moq;
using Xunit;
using ClipVault.Interfaces;

namespace ClipVault.Tests
{
    public class TagServiceTests
    {
        private readonly Mock<IAppDbContext> _mockDbContext;
        private readonly TagService _tagService;

        public TagServiceTests()
        {
            _mockDbContext = new Mock<IAppDbContext>();
            _tagService = new TagService(_mockDbContext.Object);
        }

        [Fact]
        public async Task ValidateAndCreateTagsAsync_ShouldReturnExistingTags_WhenTagsExist()
        {
            // Arrange
            var existingTags = TestDataHelper.CreateTags();
            var tagNames = existingTags.Select(t => t.Name).ToList();

            var mockTagSet = DbSetMockHelper.CreateMockDbSet(existingTags.AsQueryable());
            _mockDbContext.Setup(db => db.Tags).Returns(mockTagSet.Object);

            // Act
            var result = await _tagService.ValidateAndCreateTagsAsync(tagNames);

            // Assert
            Assert.Equal(existingTags.Count, result.Count);
            foreach (var tag in existingTags)
            {
                Assert.Contains(result, t => t.Name == tag.Name);
            }
        }

        [Fact]
        public async Task ValidateAndCreateTagsAsync_ShouldCreateNewTags_WhenTagsDoNotExist()
        {
            // Arrange
            var existingTags = TestDataHelper.CreateTags();
            var newTagNames = new List<string> { "newTag1", "newTag2" };
            var allTagNames = existingTags.Select(t => t.Name).Concat(newTagNames).ToList();

            var mockTagSet = DbSetMockHelper.CreateMockDbSet(existingTags.AsQueryable());
            _mockDbContext.Setup(db => db.Tags).Returns(mockTagSet.Object);

            // Act
            var result = await _tagService.ValidateAndCreateTagsAsync(allTagNames);

            // Assert
            Assert.Equal(allTagNames.Count, result.Count);
            foreach (var tagName in allTagNames)
            {
                Assert.Contains(result, t => t.Name == tagName);
            }
        }

        [Fact]
        public async Task ValidateAndCreateTagsAsync_ShouldHandleMixedExistingAndNewTags()
        {
            // Arrange
            var existingTags = TestDataHelper.CreateTags();
            var newTagNames = new List<string> { "newTag1", "newTag2" };
            var mixedTagNames = existingTags.Select(t => t.Name).Concat(newTagNames).ToList();

            var mockTagSet = DbSetMockHelper.CreateMockDbSet(existingTags.AsQueryable());
            _mockDbContext.Setup(db => db.Tags).Returns(mockTagSet.Object);

            // Act
            var result = await _tagService.ValidateAndCreateTagsAsync(mixedTagNames);

            // Assert
            Assert.Equal(mixedTagNames.Count, result.Count);
            foreach (var tagName in mixedTagNames)
            {
                Assert.Contains(result, t => t.Name == tagName);
            }
        }
    }
}
