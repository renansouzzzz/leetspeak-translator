public class TranslationHistoryService : ITranslationHistoryService
{
    private readonly ITranslationHistoryRepository _translationHistoryRepository;
    private readonly ILogger<TranslationHistoryService> _logger;

    public TranslationHistoryService(
        ITranslationHistoryRepository translationHistoryRepository,
        ILogger<TranslationHistoryService> logger)
    {
        _translationHistoryRepository = translationHistoryRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<Translation>> GetUserHistoryAsync(
        HistoryFilter filter,
        string userId)
    {
        try
        {
            _logger.LogInformation(
                "Fetching translation history for user {UserId} with filters: {@Filter}",
                userId, filter);

            var results = await _translationHistoryRepository.GetUserHistoryViaStoredProcAsync(filter, userId);

            _logger.LogInformation(
                "Found {Count} records for user {UserId}",
                results.Count(), userId);

            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to retrieve history for user {UserId}. Filters: {@Filter}",
                userId, filter);

            return Enumerable.Empty<Translation>();
        }
    }

    public IEnumerable<Translation> GetRecentTranslationsAsync(string userId, int count = 5)
    {
        try
        {
            _logger.LogInformation($"Fetching {count} most recent translations for user {userId}");

            var results =  _translationHistoryRepository.GetAllById(userId)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.TranslationDate)
                .Take(count)
                .ToList();

            _logger.LogDebug($"Retrieved {results.Count} recent translations");
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving recent translations for user {userId}");
            return Enumerable.Empty<Translation>();
        }
    }
}