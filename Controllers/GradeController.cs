using KidsLearn.Common;
using KidsLearn.DTOs.Grade;
using KidsLearn.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KidsLearn.Controllers;

[ApiController]
[Route("api/grades")]
[Authorize]
public class GradeController : ControllerBase
{
    private readonly IGradeService _gradeService;
    private readonly IUnitService _unitService;

    public GradeController(IGradeService gradeService, IUnitService unitService)
    {
        _gradeService = gradeService;
        _unitService = unitService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var grades = await _gradeService.GetAllAsync();
        return Ok(ApiResponse<object>.Ok(grades));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var grade = await _gradeService.GetByIdAsync(id);
        if (grade == null)
            return NotFound(ApiResponse<string>.Fail("Không tìm thấy khối lớp"));
        return Ok(ApiResponse<object>.Ok(grade));
    }

    [HttpGet("{id}/units")]
    public async Task<IActionResult> GetUnitsByGrade(int id)
    {
        var units = await _unitService.GetUnitByGradeAsync(id);
        return Ok(ApiResponse<object>.Ok(units));
    }

    // ===== ADMIN =====

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateUpdateGradeDto dto)
    {
        var created = await _gradeService.CreateAsync(dto);
        return Ok(ApiResponse<object>.Ok(created, "Đã tạo khối lớp"));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateUpdateGradeDto dto)
    {
        var updated = await _gradeService.UpdateAsync(id, dto);
        if (updated == null)
            return NotFound(ApiResponse<string>.Fail("Không tìm thấy khối lớp"));
        return Ok(ApiResponse<object>.Ok(updated, "Đã cập nhật"));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _gradeService.DeleteAsync(id);
        if (!ok) return NotFound(ApiResponse<string>.Fail("Không tìm thấy khối lớp"));
        return Ok(ApiResponse<string>.Ok("OK", "Đã xóa"));
    }
}