public interface ITranslationResultAdapter
{
    TranslationResult AdaptToResult(Translation translation);
    Translation AdaptToModel(TranslationResult result);
}