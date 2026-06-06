using KidsLearn.Common;
using KidsLearn.DTOs.Vocabulary;
using KidsLearn.Services;
using KidsLearn.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KidsLearn.Controllers;

[ApiController]
[Route("api/vocabularies")]
[Authorize]
/// Các endpoint liên quan đến từ vựng (vocabulary)
public class VocabularyController : ControllerBase
{
    private readonly IVocabularyService _vocabService;

    // Endpoint này cho phép tất cả người dùng (học sinh, giáo viên, admin) đều xem được danh sách từ vựng
    // (vocabulary) thuộc một đơn vị học tập (unit) cụ thể. 
    // Ví dụ: GET /api/vocabularies?unitId=123
    public VocabularyController(IVocabularyService vocabService)
    {
        _vocabService = vocabService;
    }
    // Endpoint này cho phép tất cả người dùng (học sinh, giáo viên, admin) đều xem được danh sách từ vựng (vocabulary)
    // thuộc một đơn vị học tập (unit) cụ thể.
    [HttpGet]
    public async Task<IActionResult> GetByUnit([FromQuery] int unitId)
    {
        var vocabs = await _vocabService.GetByUnitAsync(unitId);
        return Ok(ApiResponse<object>.Ok(vocabs));
    }

    // ===== ADMIN =====

    [HttpPost]
    [Authorize(Roles = "Admin")]
    // Endpoint này cho phép admin tạo mới một từ vựng (vocabulary) trong hệ thống. Admin sẽ gửi thông tin từ vựng
    // cùng với một file ảnh minh họa, và hệ thống sẽ lưu trữ file ảnh đó và liên kết nó với từ vựng mới được tạo.
    public async Task<IActionResult> Create([FromForm] CreateUpdateVocabDto dto)
    {
        if (dto.ImageFile == null || dto.ImageFile.Length == 0)
            return BadRequest(ApiResponse<string>.Fail("Không có file ảnh"));
        var created = await _vocabService.CreateAsync(dto);
        return Ok(ApiResponse<object>.Ok(created, "Đã thêm từ vựng"));
    }

    // Endpoint này cho phép admin cập nhật thông tin của một từ vựng đã tồn tại, bao gồm cả việc thay đổi file ảnh
    // minh họa. Admin sẽ gửi thông tin cập nhật cùng với một file ảnh mới, và hệ thống sẽ lưu trữ file ảnh mới đó và cập nhật liên kết với từ vựng.
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromForm] CreateUpdateVocabDto dto)
    {
        if (dto.ImageFile == null || dto.ImageFile.Length == 0)
            return BadRequest(ApiResponse<string>.Fail("Không có file ảnh"));
        var updated = await _vocabService.UpdateAsync(id, dto);
        if (updated == null)
            return NotFound(ApiResponse<string>.Fail("Không tìm thấy từ vựng"));
        return Ok(ApiResponse<object>.Ok(updated, "Đã cập nhật"));
    }

    // Endpoint này cho phép admin xóa một từ vựng cụ thể khỏi hệ thống.
    // Việc xóa sẽ loại bỏ hoàn toàn hồ sơ và dữ liệu liên quan đến từ vựng đó.
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _vocabService.DeleteAsync(id);
        if (!ok) return NotFound(ApiResponse<string>.Fail("Không tìm thấy từ vựng"));
        return Ok(ApiResponse<string>.Ok("OK", "Đã xóa từ vựng"));
    }
}