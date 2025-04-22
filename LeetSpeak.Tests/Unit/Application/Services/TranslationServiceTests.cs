using Moq;
using Microsoft.Extensions.Logging;
using LeetSpeak.Application.Tests.Services;

namespace LeetSpeak.Tests.Unit.Application.Services;

public class TranslationServiceTests
{
    private readonly Mock<ITranslationRepository> _mockRepo;
    private readonly Mock<ITranslationResultAdapter> _mockAdapter;
    private readonly Mock<IFunTranslationApiService> _mockApiService;
    private readonly Mock<ILogger<TranslationService>> _mockLogger;
    private readonly TranslationService _service;

    public TranslationServiceTests()
    {
        _mockRepo = new Mock<ITranslationRepository>();
        _mockAdapter = new Mock<ITranslationResultAdapter>();
        _mockApiService = new Mock<IFunTranslationApiService>();
        _mockLogger = new Mock<ILogger<TranslationService>>();

        _service = new TranslationService(
            _mockRepo.Object,
            _mockAdapter.Object,
            _mockApiService.Object,
            _mockLogger.Object);
    }

    [Fact(DisplayName = "TranslateToLeetSpeakAsync: Correctly maps API response and saves translation")]
    public async Task TranslateToLeetSpeakAsync_ShouldCorrectlyMapResponse_UnitTest()
    {
        const string text = "Hello";
        var userId = Guid.NewGuid().ToString();
        const string expectedTranslatedText = "H3ll0";

        var mockApiService = new Mock<IFunTranslationApiService>();
        mockApiService.Setup(x => x.TranslateToLeetSpeakAsync(text))
                    .ReturnsAsync(new TranslationApiResponse
                    {
                        TranslatedText = expectedTranslatedText,
                        RawResponse = "{}"
                    });

        var mockRepo = new Mock<ITranslationRepository>();
        Translation savedTranslation = null;
        mockRepo.Setup(x => x.AddAsync(It.IsAny<Translation>()))
               .Callback<Translation>(t => savedTranslation = t)
               .Returns(Task.CompletedTask);

        var mockAdapter = new Mock<ITranslationResultAdapter>();
        mockAdapter.Setup(x => x.AdaptToResult(It.IsAny<Translation>()))
                 .Returns(new TranslationResult());

        var service = new TranslationService(
            mockRepo.Object,
            mockAdapter.Object,
            mockApiService.Object,
            Mock.Of<ILogger<TranslationService>>());

        var result = await service.TranslateToLeetSpeakAsync(text, userId);

        Assert.NotNull(savedTranslation);
        Assert.Equal(expectedTranslatedText, savedTranslation.TranslatedText);
        mockRepo.Verify(x => x.AddAsync(It.IsAny<Translation>()), Times.Once);
    }

    [Fact(DisplayName = "TranslateToLeetSpeakAsync: Logs translation request and API response")]
    public async Task TranslateToLeetSpeakAsync_ShouldLogAppropriately_WhenCalled()
    {
        const string text = "Test";
        string userId = Guid.NewGuid().ToString();
        var apiResponse = new FunTranslationResponse
        {
            TranslatedText = "T3st",
            RawResponse = "{}"
        };

        _mockApiService.Setup(x => x.TranslateToLeetSpeakAsync(It.IsAny<string>()))
                      .ReturnsAsync(apiResponse);

        await _service.TranslateToLeetSpeakAsync(text, userId);

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString().Contains($"Translating text: '{text}'")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("API Response:")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Theory(DisplayName = "Rejects null, empty, or whitespace input")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task TranslateToLeetSpeakAsync_ShouldRejectInvalidInput(string invalidText)
    {
        string userId = Guid.NewGuid().ToString();

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.TranslateToLeetSpeakAsync(invalidText, userId));

        _mockApiService.Verify(x => x.TranslateToLeetSpeakAsync(It.IsAny<string>()), Times.Never);
        _mockRepo.Verify(x => x.AddAsync(It.IsAny<Translation>()), Times.Never);
    }
}