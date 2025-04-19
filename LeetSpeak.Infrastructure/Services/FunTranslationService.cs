using Newtonsoft.Json;

public class FunTranslationApiService : IFunTranslationApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<FunTranslationApiService> _logger;

    public FunTranslationApiService(HttpClient httpClient, ILogger<FunTranslationApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<TranslationApiResponse> TranslateToLeetSpeakAsync(string text)
    {
        try
        {
            var content = new FormUrlEncodedContent(
            [
                new KeyValuePair<string, string>("text", text)
            ]);

            var response = await _httpClient.PostAsync("leetspeak.json", content);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            dynamic responseObject = JsonConvert.DeserializeObject(responseString)!;

            return new TranslationApiResponse
            {
                TranslatedText = responseObject.contents.translated,
                RawResponse = responseString
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling FunTranslations API");
            throw new ApiTranslationException("Failed to translate text");
        }
    }
}