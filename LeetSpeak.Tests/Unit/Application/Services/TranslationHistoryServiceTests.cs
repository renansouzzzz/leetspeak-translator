using Microsoft.Extensions.Logging;
using Moq;

namespace LeetSpeak.Tests.Unit.Application.Services;

public class TranslationHistoryServiceTests
{
    private readonly string userId = Guid.NewGuid().ToString();
    private readonly DateTime fixedTime = new DateTime(2025, 4, 22, 10, 0, 0, DateTimeKind.Utc);
    private TranslationHistoryService _service;
    private Mock<ITranslationHistoryRepository> _mockRepo;
    private Mock<ILogger<TranslationHistoryService>> _mockLogger;

    public TranslationHistoryServiceTests()
    {
        _mockRepo = new Mock<ITranslationHistoryRepository>();
        _mockLogger = new Mock<ILogger<TranslationHistoryService>>();
        _service = new TranslationHistoryService(_mockRepo.Object, _mockLogger.Object);
    }

    private List<Translation> listTranslation => new List<Translation>
    {
        new Translation { UserId = userId, TranslationDate = fixedTime.AddHours(-1) },
        new Translation { UserId = userId, TranslationDate = fixedTime.AddMinutes(-30) },
        new Translation { UserId = userId, TranslationDate = fixedTime }
    };

    [Fact(DisplayName = "Returns recently ordered translations when valid user ID is provided")]
    public void GetRecentTranslationsAsync_ReturnsOrderedRecentItems()
    {
        _mockRepo.Setup(r => r.GetAllById(userId))
               .Returns(listTranslation.AsQueryable());

        var results = _service.GetRecentTranslationsAsync(userId, count: 2);

        Assert.Equal(fixedTime, results.First().TranslationDate);
    }
}