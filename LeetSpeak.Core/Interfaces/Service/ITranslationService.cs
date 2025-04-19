public interface ITranslationService
{
    Task<TranslationResult> TranslateToLeetSpeakAsync(string text, string userId);
}