using Microsoft.EntityFrameworkCore;

public class TranslationHistoryRepository : ITranslationHistoryRepository
{
    private readonly ApplicationDbContext _context;

    public TranslationHistoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public IQueryable<Translation> GetAllById(string userId)
    {
        return _context.Translations
        .Where(t => t.UserId == userId)
        .OrderByDescending(t => t.TranslationDate)
        .AsNoTracking();
    }

    public async Task<IEnumerable<Translation>> GetFilteredAsync(
        string userId,
        HistoryFilter filter)
    {
        var query = BuildFilterQuery(userId, filter);
        return await query.ToListAsync();
    }

    public async Task<int> CountAsync(string userId, HistoryFilter filter)
    {
        var query = BuildFilterQuery(userId, filter);
        return await query.CountAsync();
    }

    public IQueryable<Translation> BuildFilterQuery(string userId, HistoryFilter filter)
    {
        var query = _context.Translations
            .Where(t => t.UserId == userId)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            query = query.Where(t =>
                t.OriginalText.Contains(filter.SearchTerm) ||
                t.TranslatedText.Contains(filter.SearchTerm));
        }

        if (filter.StartDate.HasValue)
        {
            query = query.Where(t => t.TranslationDate >= filter.StartDate.Value);
        }

        if (filter.EndDate.HasValue)
        {
            query = query.Where(t => t.TranslationDate <= filter.EndDate.Value);
        }

        return query.OrderByDescending(t => t.TranslationDate);
    }
}