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
            return new List<Tag>
            {
                new Tag { Id = 1, Name = "example" },
                new Tag { Id = 2, Name = "test" },
            };
        }

        public static List<Tag> CreateTags(List<Tag> tags)
        {
            return tags.Select(tag => new Tag { Id = tag.Id, Name = tag.Name }).ToList();
        }
        public static Snippet CreateSnippet(SnippetCreateDto snippetDto, int languageId, List<Tag> tags)
        {
            return new Snippet
            {
                Id = 1,
                Title = snippetDto.Title,
                Code = snippetDto.Code,
                LanguageId = languageId,
                Language = new Language { Id = languageId, Name = snippetDto.Language },
                SnippetTags = tags.Select(tag => new SnippetTag
                {
                    TagId = tag.Id, // Ensure TagId is set
                    Tag = tag
                }).ToList()
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

        public static SnippetUpdateDto CreateSnippetUpdateDto(SnippetResponseDto response)
        {
            return new SnippetUpdateDto
            {
                Title = response.Title,
                Code = response.Code,
                Language = response.Language,
                TagNames = response.Tags
            };
        }

        public static List<Snippet> CreateSnippetList()
        {
            var language1 = new Language { Id = 1, Name = "C#" };
            var language2 = new Language { Id = 2, Name = "Python" };

            var tags1 = new List<Tag>
            {
                new Tag { Id = 1, Name = "example" },
                new Tag { Id = 2, Name = "test" }
            };

            var tags2 = new List<Tag>
            {
                new Tag { Id = 3, Name = "automation" },
                new Tag { Id = 4, Name = "scripting" }
            };

            var snippet1 = new Snippet
            {
                Id = 1,
                Title = "Snippet 1",
                Code = "Code 1",
                LanguageId = language2.Id,
                Language = language2,
                SnippetTags = tags2.Select(tag => new SnippetTag
                {
                    TagId = tag.Id,
                    Tag = tag
                }).ToList()
            };

            var snippet2 = new Snippet
            {
                Id = 2,
                Title = "Snippet 2",
                Code = "Code 2",
                LanguageId = language1.Id,
                Language = language1,
                SnippetTags = tags1.Select(tag => new SnippetTag
                {
                    TagId = tag.Id,
                    Tag = tag
                }).ToList()
            };

            var snippet3 = new Snippet
            {
                Id = 3,
                Title = "Snippet 3",
                Code = "Code 3",
                LanguageId = language1.Id,
                Language = language1,
                SnippetTags = new List<SnippetTag>() // Empty for AddTagsToSnippetAsync test
            };

            return new List<Snippet> { snippet1, snippet2, snippet3 };
        }

        public static void AddTagsToSnippet(List<Snippet> snippets, int snippetId, List<Tag> tagsToAdd)
        {
            var snippet = snippets.FirstOrDefault(s => s.Id == snippetId);
            if (snippet == null)
            {
                throw new ArgumentException($"Snippet with ID {snippetId} not found.");
            }

            foreach (var tag in tagsToAdd.Where(tag => !snippet.SnippetTags.Any(st => st.TagId == tag.Id)))
            {
                snippet.SnippetTags.Add(new SnippetTag { TagId = tag.Id, Tag = tag });
            }
        }
    }
}
