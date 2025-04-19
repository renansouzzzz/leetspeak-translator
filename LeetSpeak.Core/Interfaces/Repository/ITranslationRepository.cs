public interface ITranslationRepository
{
    Task AddAsync(Translation translation);
    Task<Translation?> GetByIdAsync(Guid id);
}