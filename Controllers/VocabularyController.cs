using KidsLearn.Common;
using KidsLearn.DTOs.Vocabulary;
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

    [HttpGet]
    public async Task<IActionResult> GetByUnit([FromQuery] int unitId)
    {
        var vocabs = await _vocabService.GetByUnitAsync(unitId);
        return Ok(ApiResponse<object>.Ok(vocabs));
    }

    // ===== ADMIN =====

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateUpdateVocabDto dto)
    {
        var created = await _vocabService.CreateAsync(dto);
        return Ok(ApiResponse<object>.Ok(created, "Đã thêm từ vựng"));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateUpdateVocabDto dto)
    {
        var updated = await _vocabService.UpdateAsync(id, dto);
        if (updated == null)
            return NotFound(ApiResponse<string>.Fail("Không tìm thấy từ vựng"));
        return Ok(ApiResponse<object>.Ok(updated, "Đã cập nhật"));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _vocabService.DeleteAsync(id);
        if (!ok) return NotFound(ApiResponse<string>.Fail("Không tìm thấy từ vựng"));
        return Ok(ApiResponse<string>.Ok("OK", "Đã xóa từ vựng"));
    }
}