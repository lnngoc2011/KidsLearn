using KidsLearn.Common;
using KidsLearn.DTOs.Unit;
using KidsLearn.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KidsLearn.Controllers;

[ApiController]
[Route("api/units")]
[Authorize]
/// Các endpoint liên quan đến đơn vị học tập (unit) 
/// ví dụ: Lấy thông tin chi tiết unit, Tạo mới, Cập nhật, Xóa
public class UnitController : ControllerBase
{
    private readonly IUnitService _unitService;

    public UnitController(IUnitService unitService)
    {
        _unitService = unitService;
    }

    // Để tất cả người dùng (học sinh, giáo viên, admin) đều xem được thông tin chi tiết của unit, bao gồm cả danh sách bài học (lesson) và quiz thuộc unit đó.
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var unit = await _unitService.GetUnitDetailAsync(id);
        if (unit == null)
            return NotFound(ApiResponse<string>.Fail("Không tìm thấy Unit"));
        return Ok(ApiResponse<object>.Ok(unit));
    }

    // ===== ADMIN =====

    [HttpGet]
    [Authorize(Roles = "Admin")]
    // Endpoint này cho phép admin xem được danh sách tất cả đơn vị (unit) trong hệ thống,
    // có thể lọc theo gradeId nếu muốn.    
    public async Task<IActionResult> GetAll([FromQuery] int? gradeId)
    {
        var units = await _unitService.GetAllUnitAsync(gradeId);
        return Ok(ApiResponse<object>.Ok(units));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    // Endpoint này cho phép admin tạo mới một đơn vị học tập (unit) trong hệ thống.
    public async Task<IActionResult> Create([FromForm] CreateUpdateUnitDto dto)
    {
        try
        {
            if (dto.AvatarFile == null || dto.AvatarFile.Length == 0)
                return BadRequest(ApiResponse<string>.Fail("Không có file ảnh"));
            var created = await _unitService.CreateUnitAsync(dto);
            return Ok(ApiResponse<object>.Ok(created, "Đã tạo Unit"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<string>.Fail(ex.Message));
        }
    }
    // Endpoint này cho phép admin cập nhật thông tin của một đơn vị học tập (unit) đã tồn tại trong hệ thống.
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromForm] CreateUpdateUnitDto dto)
    {

        if (dto.AvatarFile == null || dto.AvatarFile.Length == 0)
            return BadRequest(ApiResponse<string>.Fail("Không có file ảnh"));
        var updated = await _unitService.UpdateUnitAsync(id, dto);
        if (updated == null)
            return NotFound(ApiResponse<string>.Fail("Không tìm thấy Unit"));
        return Ok(ApiResponse<object>.Ok(updated, "Đã cập nhật"));
    }

    // Endpoint này cho phép admin xóa một đơn vị học tập (unit) đã tồn tại trong hệ thống.
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _unitService.DeleteUnitAsync(id);
        if (!ok) return NotFound(ApiResponse<string>.Fail("Không tìm thấy Unit"));
        return Ok(ApiResponse<string>.Ok("OK", "Đã xóa Unit (kèm Vocabulary, Quiz, Answer)"));
    }
    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<IActionResult> Search([FromQuery] string keyword)
    {
        var units = await _unitService.SearchAsync(keyword);
        return Ok(ApiResponse<object>.Ok(units));
    }
}
