using Microsoft.Extensions.Logging;
using Moq;

namespace LeetSpeak.Tests.Unit.Application.Services;

public class TranslationHistoryServiceTests
{
    private readonly string userId = Guid.NewGuid().ToString();
    private readonly string otherUserId = Guid.NewGuid().ToString();
    private readonly DateTime fixedTime = new DateTime(2025, 4, 22, 10, 0, 0, DateTimeKind.Utc);

    private readonly Mock<ITranslationHistoryRepository> _mockRepo = new();
    private readonly Mock<ILogger<TranslationHistoryService>> _mockLogger = new();
    private readonly TranslationHistoryService _service;

    public TranslationHistoryServiceTests()
    {
        _service = new TranslationHistoryService(_mockRepo.Object, _mockLogger.Object);
    }

    private List<Translation> CreateTestTranslations() => new()
    {
        new Translation { UserId = userId, TranslationDate = fixedTime.AddHours(-2), OriginalText = "Hello" },
        new Translation { UserId = userId, TranslationDate = fixedTime.AddMinutes(-30), OriginalText = "World" },
        new Translation { UserId = userId, TranslationDate = fixedTime, OriginalText = "Recent" },
        
        new Translation { UserId = otherUserId, TranslationDate = fixedTime, OriginalText = "Other" }
    };

    [Fact(DisplayName = "Returns most recent translations first when user exists")]
    public void GetRecentTranslations_ReturnsOrderedResults()
    {
        var testData = CreateTestTranslations();
        _mockRepo.Setup(r => r.GetAllById(userId))
               .Returns(testData.Where(t => t.UserId == userId).AsQueryable());

        var results = _service.GetRecentTranslationsAsync(userId, count: 2).ToList();

        Assert.Equal(2, results.Count);
        Assert.Equal("Recent", results[0].OriginalText);
        Assert.Equal("World", results[1].OriginalText);
    }

    [Fact(DisplayName = "Returns empty list when user has no translations")]
    public void GetRecentTranslations_ReturnsEmptyForNoHistory()
    {
        _mockRepo.Setup(r => r.GetAllById(userId))
               .Returns(new List<Translation>().AsQueryable());

        var results = _service.GetRecentTranslationsAsync(userId);

        Assert.Empty(results);
    }

    [Fact(DisplayName = "Returns fewer items when user history is smaller than requested count")]
    public void GetRecentTranslations_ReturnsAvailableWhenCountExceedsHistory()
    {
        var testData = CreateTestTranslations();
        _mockRepo.Setup(r => r.GetAllById(userId))
               .Returns(testData.Where(t => t.UserId == userId).AsQueryable());

        var results = _service.GetRecentTranslationsAsync(userId, count: 5);

        Assert.Equal(3, results.Count());
    }

    [Fact(DisplayName = "Logs information when retrieving translations")]
    public void GetRecentTranslations_LogsAppropriately()
    {
        var testData = CreateTestTranslations();
        _mockRepo.Setup(r => r.GetAllById(userId))
               .Returns(testData.Where(t => t.UserId == userId).AsQueryable());

        _service.GetRecentTranslationsAsync(userId, count: 2);

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains($"Fetching 2 most recent translations")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Once);
    }
}