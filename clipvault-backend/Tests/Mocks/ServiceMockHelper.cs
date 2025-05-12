using ClipVault.Dtos;
using ClipVault.Interfaces;
using ClipVault.Models;
using Moq;
using System.Collections.Generic;

namespace ClipVault.Tests.Mocks
{
    public static class ServiceMockHelper
    {
        public static Mock<ITagService> CreateMockTagService(List<Tag> tags, List<string> tagNames)
        {
            var mockTagService = new Mock<ITagService>();
            mockTagService.Setup(ts => ts.ValidateAndCreateTagsAsync(tagNames)).ReturnsAsync(tags);
            return mockTagService;
        }

        public static Mock<ISnippetMapper> CreateMockSnippetMapper(SnippetCreateDto snippetDto, Language language, List<Tag> tags, Snippet snippet)
        {
            var mockSnippetMapper = new Mock<ISnippetMapper>();
            mockSnippetMapper.Setup(sm => sm.MapToSnippetEntity(snippetDto, language.Id, tags)).Returns(snippet);
            mockSnippetMapper.Setup(sm => sm.MapToSnippetResponseDto(snippet)).Returns(new SnippetResponseDto
            {
                Id = snippet.Id,
                Title = snippet.Title,
                Code = snippet.Code,
                Language = language.Name,
                Tags = snippetDto.TagNames
            });
            return mockSnippetMapper;
        }
    }
}
