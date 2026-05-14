using KidsLearn.Common;
using KidsLearn.DTOs.Vocabulary;
using KidsLearn.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KidsLearn.Controllers.Admin;

[ApiController]
[Route("api/admin/vocabularies")]
[Authorize(Policy = "AdminOnly")]
public class AdminVocabController : ControllerBase
{
    private readonly IVocabularyService _vocabService;

    public AdminVocabController(IVocabularyService vocabService)
    {
        _vocabService = vocabService;
    }

    [HttpGet]
    public async Task<IActionResult> GetByUnit([FromQuery] int unitId)
    {
        var vocabs = await _vocabService.GetByUnitAsync(unitId);
        return Ok(ApiResponse<object>.Ok(vocabs));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUpdateVocabDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<string>.Fail("Dữ liệu không hợp lệ"));

        try
        {
            var result = await _vocabService.CreateAsync(dto);
            return Ok(ApiResponse<object>.Ok(result, "Tạo từ vựng thành công"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<string>.Fail(ex.Message));
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateUpdateVocabDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<string>.Fail("Dữ liệu không hợp lệ"));

        var result = await _vocabService.UpdateAsync(id, dto);
        if (result == null)
            return NotFound(ApiResponse<string>.Fail("Không tìm thấy từ vựng"));
        return Ok(ApiResponse<object>.Ok(result, "Cập nhật thành công"));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _vocabService.DeleteAsync(id);
        if (!ok)
            return NotFound(ApiResponse<string>.Fail("Không tìm thấy từ vựng"));
        return Ok(ApiResponse<string>.Ok("OK", "Xóa thành công"));
    }
}