using ClipVault.Models;
using ClipVault.Dtos;

namespace ClipVault.Tests.Mocks
{
    public static class MockDataFactory
    {
        public static List<Language> CreateLanguages()
        {
            return new List<Language>
            {
                new Language { Id = 1, Name = "C#" },
                new Language { Id = 2, Name = "Python" }
            };
        }

        public static List<Snippet> CreateSnippets(List<Language> languages)
        {
            var trashedSnippet = new Snippet
            {
                Id = 1,
                Title = "Trashed Snippet",
                Code = "Sample code 1",
                IsDeleted = true,
                LanguageId = 1,
                Language = languages[0],
                SnippetTags = new List<SnippetTag>(),
                Collection = null,
                CollectionId = null
            };
            var activeSnippet = new Snippet
            {
                Id = 2,
                Title = "Active Snippet",
                Code = "Sample code 2",
                IsDeleted = false,
                LanguageId = 2,
                Language = languages[1],
                SnippetTags = new List<SnippetTag>()
            };
            return new List<Snippet> { trashedSnippet, activeSnippet };
        }

        public static List<Collection> CreateCollections(List<Snippet> snippets)
        {
            var trashedCollection = new Collection
            {
                Id = 1,
                Name = "Trashed Collection",
                IsDeleted = true,
                Snippets = new List<Snippet>(),
                SubCollections = new List<Collection>(),
                ParentCollectionId = null
            };
            var activeSnippet = snippets.Find(s => s.Id == 2);
            var activeCollection = new Collection
            {
                Id = 2,
                Name = "Active Collection",
                IsDeleted = false,
                Snippets = new List<Snippet> { activeSnippet },
                SubCollections = new List<Collection>(),
                ParentCollectionId = null
            };
            if (activeSnippet != null)
            {
                activeSnippet.Collection = activeCollection;
                activeSnippet.CollectionId = activeCollection.Id;
            }
            return new List<Collection> { trashedCollection, activeCollection };
        }

        public static List<Tag> CreateTags()
        {
            // Example tags for use in snippet tests
            return new List<Tag>
            {
                new Tag { Id = 1, Name = "example" },
                new Tag { Id = 2, Name = "automation" },
                new Tag { Id = 3, Name = "scripting" },
                new Tag { Id = 4, Name = "test" }
            };
        }

        public static SnippetCreateDto CreateSnippetCreateDto()
        {
            return new SnippetCreateDto
            {
                Title = "Active Snippet",
                Code = "Sample code 2",
                Language = "Python",
                TagNames = new List<string> { "example" }
            };
        }

        public static SnippetUpdateDto CreateSnippetUpdateDto()
        {
            return new SnippetUpdateDto
            {
                Title = "Updated Title",
                Code = "Updated Code"
            };
        }

        public static SnippetResponseDto CreateSnippetResponseDto()
        {
            return new SnippetResponseDto
            {
                Id = 2,
                Title = "Active Snippet",
                Code = "Sample code 2",
                Language = "Python",
                Tags = new List<string> { "example" }
            };
        }

        public static List<SnippetResponseDto> CreateSnippetResponseDtoList(int count = 1)
        {
            var list = new List<SnippetResponseDto>();
            for (int i = 0; i < count; i++)
            {
                list.Add(CreateSnippetResponseDto());
            }
            return list;
        }

        public static Snippet CreateActiveSnippetWithTags()
        {
            var language = CreateLanguages().First(l => l.Name == "Python");
            var tags = CreateTags();
            return new Snippet
            {
                Id = 2,
                Title = "Active Snippet",
                Code = "Sample code 2",
                IsDeleted = false,
                LanguageId = language.Id,
                Language = language,
                SnippetTags = tags.Select(t => new SnippetTag { Tag = t, TagId = t.Id }).ToList()
            };
        }

        public static User CreateUser()
        {
            return new User
            {
                UserName = "testuser",
                Email = "test@example.com",
                PasswordHash = "hashedpassword"
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

        public static RefreshTokenDto CreateRefreshTokenDto(string token = "valid-refresh-token")
        {
            return new RefreshTokenDto
            {
                RefreshToken = token
            };
        }

        public static CollectionDto CreateActiveCollectionDto()
        {
            return new CollectionDto
            {
                Id = 2,
                Name = "Active Collection",
                ParentCollectionId = null,
                SubCollections = new List<CollectionDto>(),
                Snippets = new List<SnippetResponseDto>()
            };
        }

        public static List<CollectionDto> CreateCollectionDtoList()
        {
            return new List<CollectionDto> { CreateActiveCollectionDto() };
        }
    }
}
