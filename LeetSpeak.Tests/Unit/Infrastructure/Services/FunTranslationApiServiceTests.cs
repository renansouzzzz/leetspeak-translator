using Microsoft.Extensions.Logging;
using Moq.Protected;
using Moq;
using System.Net;

namespace LeetSpeak.Tests.Unit.Infrastructure.Services;

public class FunTranslationApiServiceTests
{
    [Fact(DisplayName = "Returns translated text on successful API response (200)")]
    public async Task TranslateToLeetSpeakAsync_ReturnsTranslatedText()
    {
        var expectedTranslation = "h3ll0";
        var jsonResponse = @$"{{
            ""contents"": {{
                ""translated"": ""{expectedTranslation}""
            }}
        }}";

        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse),
            });

        var httpClient = new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("https://api.funtranslations.com/")
        };

        var loggerMock = new Mock<ILogger<FunTranslationApiService>>();
        var service = new FunTranslationApiService(httpClient, loggerMock.Object);

        var result = await service.TranslateToLeetSpeakAsync("hello");

        Assert.NotNull(result);
        Assert.Equal(expectedTranslation, result.TranslatedText);
        Assert.Contains(expectedTranslation, result.RawResponse);
    }

    [Fact(DisplayName = "Throws ApiTranslationException on API server error (500)")]
    public async Task TranslateToLeetSpeakAsync_WhenApiFails_ThrowsApiTranslationException()
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Content = new StringContent("Server error")
            });

        var httpClient = new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("https://api.funtranslations.com/")
        };

        var loggerMock = new Mock<ILogger<FunTranslationApiService>>();
        var service = new FunTranslationApiService(httpClient, loggerMock.Object);

        var ex = await Assert.ThrowsAsync<ApiTranslationException>(() =>
            service.TranslateToLeetSpeakAsync("hello"));

        Assert.Equal("Failed to translate text", ex.Message);
    }
}
