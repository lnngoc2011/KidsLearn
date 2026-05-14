using KidsLearn.Common;
using KidsLearn.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KidsLearn.Controllers;

[ApiController]
[Route("api/vocabularies")]
[Authorize]
public class VocabularyController : ControllerBase
{
    private readonly IVocabularyService _vocabService;

    public VocabularyController(IVocabularyService vocabService)
    {
        _vocabService = vocabService;
    }

    /// <summary>
    /// Lấy danh sách từ vựng theo UnitId
    /// GET /api/vocabularies?unitId=5
    /// ✨ FIX: param lessonId → unitId
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetByUnit([FromQuery] int unitId)
    {
        var vocabs = await _vocabService.GetByUnitAsync(unitId);
        return Ok(ApiResponse<object>.Ok(vocabs));
    }
}