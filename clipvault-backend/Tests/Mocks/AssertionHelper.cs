using ClipVault.Dtos;
using Xunit;
using System.Collections.Generic;

namespace ClipVault.Tests.Mocks
{
    public static class AssertionHelper
    {
        public static void AssertSnippetResponseDto(SnippetResponseDto actual, SnippetCreateDto expected, string expectedLanguage, List<string> expectedTags)
        {
            Assert.NotNull(actual);
            Assert.Equal(expected.Title, actual.Title);
            Assert.Equal(expected.Code, actual.Code);
            Assert.Equal(expected.Language, actual.Language);
            Assert.Equal(expectedTags, actual.Tags);
        }
    }
}
