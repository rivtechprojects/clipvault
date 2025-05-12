using ClipVault.Dtos;
using ClipVault.Models;

namespace ClipVault.Tests.Mocks
{
    public static class TestDataHelper
    {
        public static SnippetCreateDto CreateSnippetCreateDto()
        {
            return new SnippetCreateDto
            {
                Title = "Test Snippet",
                Code = "Console.WriteLine(\"Hello World\");",
                Language = "C#",
                TagNames = ["example", "test"]
            };
        }

        public static Language CreateLanguage()
        {
            return new Language { Id = 1, Name = "C#" };
        }

        public static List<Tag> CreateTags()
        {
            return
            [
                new Tag { Id = 1, Name = "example" },
                new Tag { Id = 2, Name = "test" }
            ];
        }

        public static Snippet CreateSnippet(SnippetCreateDto snippetDto, Language language)
        {
            return new Snippet
            {
                Id = 1,
                Title = snippetDto.Title,
                Code = snippetDto.Code,
                LanguageId = language.Id
            };
        }
    }
}
