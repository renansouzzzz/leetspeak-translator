public interface ITranslationHistoryRepository
{
    Task<IEnumerable<Translation>> GetFilteredAsync(
        string userId,
        HistoryFilter filter);
    Task<int> CountAsync(string userId, HistoryFilter filter);
    IQueryable<Translation> BuildFilterQuery(string userId, HistoryFilter filter);
    IQueryable<Translation> GetAllById(string userId);
}