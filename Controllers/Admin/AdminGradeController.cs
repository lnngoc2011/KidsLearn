using KidsLearn.Common;
using KidsLearn.DTOs.Grade;
using KidsLearn.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KidsLearn.Controllers.Admin;

[ApiController]
[Route("api/admin/grades")]
[Authorize(Policy = "AdminOnly")]   // Chỉ Admin truy cập
public class AdminGradeController : ControllerBase
{
    private readonly IGradeService _gradeService;

    public AdminGradeController(IGradeService gradeService)
    {
        _gradeService = gradeService;
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
            return NotFound(ApiResponse<string>.Fail("Không tìm thấy Grade"));
        return Ok(ApiResponse<object>.Ok(grade));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUpdateGradeDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<string>.Fail("Dữ liệu không hợp lệ"));

        var result = await _gradeService.CreateAsync(dto);
        return Ok(ApiResponse<object>.Ok(result, "Tạo Grade thành công"));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateUpdateGradeDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<string>.Fail("Dữ liệu không hợp lệ"));

        var result = await _gradeService.UpdateAsync(id, dto);
        if (result == null)
            return NotFound(ApiResponse<string>.Fail("Không tìm thấy Grade"));
        return Ok(ApiResponse<object>.Ok(result, "Cập nhật thành công"));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var ok = await _gradeService.DeleteAsync(id);
            if (!ok)
                return NotFound(ApiResponse<string>.Fail("Không tìm thấy Grade"));
            return Ok(ApiResponse<string>.Ok("OK", "Xóa thành công"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<string>.Fail(ex.Message));
        }
    }
}