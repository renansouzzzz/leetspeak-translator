public interface ITranslationHistoryRepository
{
    Task<IEnumerable<Translation>> GetUserHistoryViaStoredProcAsync(
        HistoryFilter filter,
        string userId);
    IQueryable<Translation> GetAllById(string userId);
}