public class TranslationResultAdapter : ITranslationResultAdapter
{
    public TranslationResult AdaptToResult(Translation translation)
    {
        return new TranslationResult()
        {
            OriginalText = translation.OriginalText,
            TranslatedText = translation.TranslatedText,
            TranslationDate = translation.TranslationDate
        };
    }
    public Translation AdaptToModel(TranslationResult result)
    {
        return new Translation()
        {
            OriginalText = result.OriginalText,
            TranslatedText = result.TranslatedText,
            TranslationDate = result.TranslationDate,
        };
    }
}