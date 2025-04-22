using Microsoft.Extensions.Logging;
using Moq;

public class TranslationHistoryServiceTests
{
    private readonly Mock<ITranslationHistoryRepository> _mockRepo;
    private readonly Mock<ILogger<TranslationHistoryService>> _mockLogger;
    private readonly TranslationHistoryService _service;

    public TranslationHistoryServiceTests()
    {
        _mockRepo = new Mock<ITranslationHistoryRepository>();
        _mockLogger = new Mock<ILogger<TranslationHistoryService>>();
        _service = new TranslationHistoryService(_mockRepo.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetUserHistoryAsync_ReturnsResults_WhenRepositoryReturnsData()
    {
        var userId = Guid.NewGuid().ToString();
        var filter = new HistoryFilter { SearchTerm = "test" };
        var fakeResult = new List<Translation>
        {
            new Translation { OriginalText = "hello", TranslatedText = "h3ll0", UserId = userId }
        };

        _mockRepo.Setup(r => r.GetUserHistoryViaStoredProcAsync(filter, userId))
                 .ReturnsAsync(fakeResult);

        var result = await _service.GetUserHistoryAsync(filter, userId);

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("hello", result.First().OriginalText);
    }
}
