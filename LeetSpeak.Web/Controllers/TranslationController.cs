using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Authorize]
public class TranslateController : Controller
{
    private readonly ITranslationService _translationService;
    private readonly ITranslationHistoryService _historyService;
    private readonly ILogger<TranslateController> _logger;

    public TranslateController(
        ITranslationService translationService,
        ITranslationHistoryService historyService,
        ILogger<TranslateController> logger)
    {
        _translationService = translationService;
        _historyService = historyService;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Translate([FromBody] TranslationRequest request)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _translationService.TranslateToLeetSpeakAsync(request.Text, userId!);

            return Json(new
            {
                success = true,
                original = result.OriginalText,
                translated = result.TranslatedText,
                date = result.TranslationDate.ToString("g")
            });
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante a tradução");
            return Json(new { success = false, error = "Erro interno durante a tradução" });
        }
    }

    [HttpGet]
    public IActionResult GetRecentHistory()
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var items = _historyService.GetRecentTranslationsAsync(userId!);

            return Json(new
            {
                items
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting history");
            return Json(new { error = "Error loading history" });
        }
    }
}