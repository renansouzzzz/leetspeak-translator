public interface IFunTranslationApiService
{
    /// <summary>
    /// Translates text to leetspeak using an external API.
    /// </summary>
    /// <param name="text">The original text to be translated.</param>
    /// <returns>API response containing the translated text and the raw response.</returns>
    /// <exception cref="ApiTranslationException">Thrown when an error occurs during the API call.</exception>
    Task<TranslationApiResponse> TranslateToLeetSpeakAsync(string text);
}

/// <summary>
/// Model representing the response from the translation API.
/// </summary>
public class TranslationApiResponse
{
    public string TranslatedText { get; set; }
    public string RawResponse { get; set; }
}

/// <summary>
/// Custom exception for translation errors via the API.
/// </summary>
public class ApiTranslationException : Exception
{
    public ApiTranslationException(string message) : base(message) { }

    public ApiTranslationException(string message, Exception innerException)
        : base(message, innerException) { }
}
