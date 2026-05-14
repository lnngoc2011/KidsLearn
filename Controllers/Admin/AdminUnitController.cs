using KidsLearn.Common;
using KidsLearn.DTOs.Unit;
using KidsLearn.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KidsLearn.Controllers.Admin;

[ApiController]
[Route("api/admin/units")]
[Authorize(Policy = "AdminOnly")]
public class AdminUnitController : ControllerBase
{
    private readonly IUnitService _unitService;

    public AdminUnitController(IUnitService unitService)
    {
        _unitService = unitService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var units = await _unitService.GetAllUnitAsync();
        return Ok(ApiResponse<object>.Ok(units));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var unit = await _unitService.GetUnitDetailAsync(id);
        if (unit == null)
            return NotFound(ApiResponse<string>.Fail("Không tìm thấy Unit"));
        return Ok(ApiResponse<object>.Ok(unit));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUpdateUnitDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<string>.Fail("Dữ liệu không hợp lệ"));

        try
        {
            var result = await _unitService.CreateUnitAsync(dto);
            return Ok(ApiResponse<object>.Ok(result, "Tạo Unit thành công"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<string>.Fail(ex.Message));
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateUpdateUnitDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<string>.Fail("Dữ liệu không hợp lệ"));

        var result = await _unitService.UpdateUnitAsync(id, dto);
        if (result == null)
            return NotFound(ApiResponse<string>.Fail("Không tìm thấy Unit"));
        return Ok(ApiResponse<object>.Ok(result, "Cập nhật thành công"));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _unitService.DeleteUnitAsync(id);
        if (!ok)
            return NotFound(ApiResponse<string>.Fail("Không tìm thấy Unit"));
        return Ok(ApiResponse<string>.Ok("OK", "Xóa thành công"));
    }
}