public class TranslationRepository : ITranslationRepository
{
    private readonly ApplicationDbContext _context;

    public TranslationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Translation entity)
    {
        await _context.Translations.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<Translation?> GetByIdAsync(Guid id)
    {
        return await _context.Translations.FindAsync(id);
    }
}