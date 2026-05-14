using KidsLearn.Common;
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

    /// <summary>
    /// Lấy chi tiết Unit kèm danh sách Vocabulary và Quiz
    /// GET /api/units/{id}
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var unit = await _unitService.GetUnitDetailAsync(id);
        if (unit == null)
            return NotFound(ApiResponse<string>.Fail("Không tìm thấy Unit"));
        return Ok(ApiResponse<object>.Ok(unit));
    }
}