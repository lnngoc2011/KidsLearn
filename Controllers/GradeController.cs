using KidsLearn.Common;
using KidsLearn.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KidsLearn.Controllers;

[ApiController]
[Route("api/grades")]
[Authorize]   // Yêu cầu đăng nhập
public class GradeController : ControllerBase
{
    private readonly IGradeService _gradeService;
    private readonly IUnitService _unitService;

    public GradeController(IGradeService gradeService, IUnitService unitService)
    {
        _gradeService = gradeService;
        _unitService = unitService;
    }

    /// <summary>
    /// Lấy danh sách 5 khối lớp (Lớp 1-5)
    /// GET /api/grades
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var grades = await _gradeService.GetAllAsync();
        return Ok(ApiResponse<object>.Ok(grades));
    }

    /// <summary>
    /// Lấy 1 Grade theo ID
    /// GET /api/grades/{id}
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var grade = await _gradeService.GetByIdAsync(id);
        if (grade == null)
            return NotFound(ApiResponse<string>.Fail("Không tìm thấy khối lớp"));
        return Ok(ApiResponse<object>.Ok(grade));
    }

    /// <summary>
    /// Lấy danh sách Unit của Grade
    /// GET /api/grades/{id}/units
    /// ✨ MỚI: API quan trọng cho học sinh
    /// </summary>
    [HttpGet("{id}/units")]
    public async Task<IActionResult> GetUnitsByGrade(int id)
    {
        var units = await _unitService.GetUnitByGradeAsync(id);
        return Ok(ApiResponse<object>.Ok(units));
    }
}