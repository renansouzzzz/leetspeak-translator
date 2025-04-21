using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Authorize]
public class HistoryController : Controller
{
    private readonly ITranslationHistoryService _historyService;
    private readonly ILogger<TranslateController> _logger;

    public HistoryController(
        ITranslationHistoryService historyService,
        ILogger<TranslateController> logger)
    {
        _historyService = historyService;
        _logger = logger;
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetHistory(
    [FromQuery] HistoryFilter filter,
    [FromQuery] int pageSize = 10,
    CancellationToken ct = default)
    {
        filter.PageSize = pageSize;

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await _historyService.GetUserHistoryAsync(filter, userId!);

        return Ok(result);
    }

    [HttpGet]
    public IActionResult Filter()
    {
        return View(new HistoryFilterViewModel
        {
            StartDate = DateTime.Now.AddDays(-7),
            PageSize = 10
        });
    }
}