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
                TagNames = new List<string> { "example", "test" }
            };
        }

        public static SnippetResponseDto CreateSnippetResponseDto()
        {
            return new SnippetResponseDto
            {
                Id = 1,
                Title = "Test Snippet",
                Code = "Console.WriteLine(\"Hello World\");",
                Language = "C#",
                Tags = new List<string> { "example", "test" }
            };
        }

        public static SnippetResponseDto CreateSnippetResponseDto(Snippet snippet)
        {
            return new SnippetResponseDto
            {
                Id = snippet.Id,
                Title = snippet.Title,
                Code = snippet.Code,
                Language = snippet.Language?.Name ?? "",
                Tags = snippet.SnippetTags.Select(st => st.Tag?.Name ?? "").ToList()
            };
        }

        public static List<SnippetResponseDto> CreateSnippetResponseDtoList(int count = 1)
        {
            var snippets = new List<SnippetResponseDto>();
            for (int i = 0; i < count; i++)
            {
                snippets.Add(new SnippetResponseDto
                {
                    Id = i + 1,
                    Title = $"Test Snippet {i + 1}",
                    Code = $"Console.WriteLine(\"Hello World {i + 1}\");",
                    Language = "C#",
                    Tags = new List<string> { "example", "test" }
                });
            }
            return snippets;
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

        public static RegisterDto CreateRegisterDto()
        {
            return new RegisterDto
            {
                UserName = "testuser",
                Email = "test@example.com",
                Password = "password123"
            };
        }

        public static LoginDto CreateLoginDto()
        {
            return new LoginDto
            {
                UserNameOrEmail = "testuser",
                Password = "password123"
            };
        }

        public static User CreateUser(RegisterDto? registerDto = null)
        {
            registerDto ??= CreateRegisterDto();
            return new User
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email,
                PasswordHash = "hashedpassword"
            };
        }
    }
}
