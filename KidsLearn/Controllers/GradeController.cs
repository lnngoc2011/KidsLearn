using KidsLearn.Common;
using KidsLearn.DTOs.Grade;
using KidsLearn.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KidsLearn.Controllers;


[ApiController]
[Route("api/grades")]
[Authorize]
///Các endpoint liên quan đến khối lớp (grade) — ví dụ: Lấy danh sách khối lớp,
///Lấy thông tin chi tiết khối lớp, Tạo mới, Cập nhật, Xóa
public class GradeController : ControllerBase
{
    private readonly IGradeService _gradeService;
    private readonly IUnitService _unitService;
    /// Để tách biệt rõ ràng giữa GradeService và UnitService, tránh việc GradeService 
    /// phải phụ thuộc vào UnitService để lấy thông tin đơn vị (unit) khi cần thiết.
    public GradeController(IGradeService gradeService, IUnitService unitService)
    {
        _gradeService = gradeService;
        _unitService = unitService;
    }

    /// Để tất cả người dùng (học sinh, giáo viên, admin) đều có thể xem được danh sách khối lớp hiện có trong hệ thống. 
    /// Đây là thông tin cơ bản và không nhạy cảm, nên không cần phân quyền đặc biệt.

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var grades = await _gradeService.GetAllAsync();
        return Ok(ApiResponse<object>.Ok(grades));
    }
    // Để tất cả người dùng (học sinh, giáo viên, admin) đều có thể xem được thông tin chi tiết của khối lớp,
    // bao gồm cả danh sách đơn vị (unit) thuộc khối lớp đó. Đây là thông tin cơ bản và không nhạy cảm, nên không cần phân quyền đặc biệt.
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var grade = await _gradeService.GetByIdAsync(id);
        if (grade == null)
            return NotFound(ApiResponse<string>.Fail("Không tìm thấy khối lớp"));
        return Ok(ApiResponse<object>.Ok(grade));
    }

    // Endpoint này cho phép người dùng (học sinh, giáo viên, admin) xem được danh sách đơn vị (unit) thuộc khối lớp cụ thể.
    // Đây là thông tin cơ bản và không nhạy cảm, nên không cần phân quyền đặc biệt.
    [HttpGet("{id}/units")]
    public async Task<IActionResult> GetUnitsByGrade(int id)
    {
        var units = await _unitService.GetUnitByGradeAsync(id);
        return Ok(ApiResponse<object>.Ok(units));
    }

    // ===== ADMIN =====

    [HttpPost]
    [Authorize(Roles = "Admin")]

    // Endpoint này chỉ dành cho admin để tạo mới khối lớp.
    // Việc tạo khối lớp là một hành động quản trị, nên cần phân quyền rõ ràng.
    public async Task<IActionResult> Create([FromBody] CreateUpdateGradeDto dto)
    {
        var created = await _gradeService.CreateAsync(dto);
        return Ok(ApiResponse<object>.Ok(created, "Đã tạo khối lớp"));
    }

    // Endpoint này chỉ dành cho admin để cập nhật thông tin khối lớp.
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateUpdateGradeDto dto)
    {
        var updated = await _gradeService.UpdateAsync(id, dto);
        if (updated == null)
            return NotFound(ApiResponse<string>.Fail("Không tìm thấy khối lớp"));
        return Ok(ApiResponse<object>.Ok(updated, "Đã cập nhật"));
    }

    // Endpoint này chỉ dành cho admin để xóa khối lớp.
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _gradeService.DeleteAsync(id);
        if (!ok) return NotFound(ApiResponse<string>.Fail("Không tìm thấy khối lớp"));
        return Ok(ApiResponse<string>.Ok("OK", "Đã xóa"));
    }
}