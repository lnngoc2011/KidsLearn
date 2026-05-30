using KidsLearn.Common;
using KidsLearn.DTOs.Unit;
using KidsLearn.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KidsLearn.Controllers;

[ApiController]
[Route("api/units")]
[Authorize]
public class UnitController : ControllerBase
{
    private readonly IUnitService _unitService;

    public UnitController(IUnitService unitService)
    {
        _unitService = unitService;
    }

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
    public async Task<IActionResult> GetAll([FromQuery] int? gradeId)
    {
        var units = await _unitService.GetAllUnitAsync(gradeId);
        return Ok(ApiResponse<object>.Ok(units));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateUpdateUnitDto dto)
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

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateUpdateUnitDto dto)
    {

        if (dto.AvatarFile == null || dto.AvatarFile.Length == 0)
            return BadRequest(ApiResponse<string>.Fail("Không có file ảnh"));
        var updated = await _unitService.UpdateUnitAsync(id, dto);
        if (updated == null)
            return NotFound(ApiResponse<string>.Fail("Không tìm thấy Unit"));
        return Ok(ApiResponse<object>.Ok(updated, "Đã cập nhật"));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _unitService.DeleteUnitAsync(id);
        if (!ok) return NotFound(ApiResponse<string>.Fail("Không tìm thấy Unit"));
        return Ok(ApiResponse<string>.Ok("OK", "Đã xóa Unit (kèm Vocabulary, Quiz, Answer)"));
    }
}