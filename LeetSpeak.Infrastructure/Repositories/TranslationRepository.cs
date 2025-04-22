using Microsoft.EntityFrameworkCore;

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
        _context.ChangeTracker.Clear();
        await _context.SaveChangesAsync();
    }
}