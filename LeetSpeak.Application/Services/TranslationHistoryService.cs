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

    public IEnumerable<Translation> GetUserHistoryAsync(HistoryFilter filter, string userId)
    {
        try
        {
            _logger.LogInformation($"Fetching translation history for user {userId} with filters: " +
                $"Search='{filter.SearchTerm}', StartDate={filter.StartDate}, EndDate={filter.EndDate}");

            var query = _translationHistoryRepository.GetAllById(userId)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.TranslationDate);

            if (!string.IsNullOrEmpty(filter.SearchTerm))
            {
                query = (IOrderedQueryable<Translation>)query.Where(t =>
                    t.OriginalText.Contains(filter.SearchTerm) ||
                    t.TranslatedText.Contains(filter.SearchTerm));

                _logger.LogDebug($"Applied search filter: {filter.SearchTerm}");
            }

            if (filter.StartDate.HasValue)
            {
                query = (IOrderedQueryable<Translation>)query.Where(t => t.TranslationDate >= filter.StartDate.Value.Date);
                _logger.LogDebug($"Applied start date filter: {filter.StartDate.Value.Date}");
            }

            if (filter.EndDate.HasValue)
            {
                var endOfDay = filter.EndDate.Value.Date.AddDays(1).AddTicks(-1);
                query = (IOrderedQueryable<Translation>)query.Where(t => t.TranslationDate <= endOfDay);
                _logger.LogDebug($"Applied end date filter: {endOfDay}");
            }

            var results = query.ToList();
            _logger.LogInformation($"Found {results.Count} history items for user {userId}");

            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving history for user {userId}");
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