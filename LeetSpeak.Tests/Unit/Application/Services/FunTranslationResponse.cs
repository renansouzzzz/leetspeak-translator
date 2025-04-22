namespace LeetSpeak.Application.Tests.Services
{
    internal class FunTranslationResponse : TranslationApiResponse
    {
        public string TranslatedText { get; set; }
        public string RawResponse { get; set; }
    }
}