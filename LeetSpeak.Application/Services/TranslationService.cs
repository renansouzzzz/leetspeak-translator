public class TranslationService : ITranslationService
{
    private readonly ITranslationRepository _translationRepository;
    private readonly ITranslationResultAdapter _adapter;
    private readonly IFunTranslationApiService _apiService;
    private readonly ILogger<TranslationService> _logger;

    public TranslationService(
        ITranslationRepository translationRepository,
        ITranslationResultAdapter adapter,
        IFunTranslationApiService apiService,
        ILogger<TranslationService> logger)
    {
        _translationRepository = translationRepository;
        _adapter = adapter;
        _apiService = apiService;
        _logger = logger;
    }

    public async Task<TranslationResult> TranslateToLeetSpeakAsync(string text, string userId)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Text cannot be null or empty", nameof(text));

        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User ID cannot be null or empty", nameof(userId));

        _logger.LogInformation($"Translating text: '{text}' for user {userId}");

        var apiResponse = await _apiService.TranslateToLeetSpeakAsync(text);

        _logger.LogInformation($"API Response: {apiResponse.RawResponse}");

        var translation = new Translation
        {
            OriginalText = text,
            TranslatedText = apiResponse.TranslatedText,
            TranslationDate = DateTime.UtcNow,
            UserId = userId
        };

        await _translationRepository.AddAsync(translation);

        return _adapter.AdaptToResult(translation);
    }
}