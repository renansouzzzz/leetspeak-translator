public interface ITranslationHistoryService
{
    IEnumerable<Translation> GetUserHistoryAsync(HistoryFilter filter, string userId);

    IEnumerable<Translation> GetRecentTranslationsAsync(string userId, int count = 5);
}