using System.Data;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Dapper;

public class TranslationHistoryRepository : ITranslationHistoryRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _config;

    public TranslationHistoryRepository(ApplicationDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public IQueryable<Translation> GetAllById(string userId)
    {
        return _context.Translations
        .AsNoTracking()
        .Where(t => t.UserId == userId)
        .OrderByDescending(t => t.TranslationDate);
    }

    public async Task<IEnumerable<Translation>> GetUserHistoryViaStoredProcAsync(
        HistoryFilter filter,
        string userId)
    {
        await using var connection = new MySqlConnection(_config.GetConnectionString("DefaultConnection"));

        return await connection.QueryAsync<Translation>(
            "GetFilteredTranslations",
            new
            {
                p_UserId = userId,
                p_SearchTerm = string.IsNullOrEmpty(filter.SearchTerm) ? null : filter.SearchTerm,
                p_StartDate = filter.StartDate,
                p_EndDate = filter.EndDate
            },
            commandType: CommandType.StoredProcedure
        );
    }
}