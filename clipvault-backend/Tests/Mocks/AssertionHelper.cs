using ClipVault.Dtos;
using ClipVault.Models;
using Xunit;

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

        public static void AssertUser(User actual, User expected)
        {
            Assert.NotNull(actual);
            Assert.Equal(expected.UserName, actual.UserName);
            Assert.Equal(expected.Email, actual.Email);
            Assert.Equal(expected.PasswordHash, actual.PasswordHash);
        }
    }
}
