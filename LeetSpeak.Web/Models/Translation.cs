public class Translation
{
    public int Id { get; set; }
    public string OriginalText { get; set; }
    public string TranslatedText { get; set; }
    public DateTime TranslationDate { get; set; }
    public string? ApiResponse { get; set; }
    public string? UserId { get; set; }
    public ApplicationUser? User { get; set; }
}