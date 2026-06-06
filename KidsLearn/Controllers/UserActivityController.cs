using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using KidsLearn.DTOs;
using KidsLearn.Services;

namespace KidsLearn.Controllers;

[ApiController]
[Route("api/activity")]
[Authorize]
// Các endpoint liên quan đến hoạt động học tập của người dùng (user activity)
public class UserActivityController : ControllerBase
{
    private readonly IUserActivityService _service;

    public UserActivityController(IUserActivityService service)
    {
        _service = service;
    }

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    // PUT /api/activity/me — Lưu vị trí học mới
    [HttpPut("me")]
    public async Task<IActionResult> UpdateMyActivity([FromBody] UpdateActivityDto dto)
    {
        var userId = GetUserId();
        await _service.UpdateActivityAsync(userId, dto);
        return Ok(new { success = true, message = "Đã cập nhật vị trí học" });
    }

    // GET /api/activity/me/unit/{unitId} — Lấy vị trí học của Unit cụ thể
    [HttpGet("me/unit/{unitId}")]
    public async Task<IActionResult> GetByUnit(int unitId)
    {
        var userId = GetUserId();
        var data = await _service.GetByUnitAsync(userId, unitId);
        return Ok(new { success = true, data });
    }

    // GET /api/activity/me/in-progress — Danh sách Unit đang học dở
    [HttpGet("me/in-progress")]
    public async Task<IActionResult> GetInProgress()
    {
        var userId = GetUserId();
        var data = await _service.GetInProgressAsync(userId);
        return Ok(new { success = true, data });
    }

    // GET /api/activity/me/latest — Unit truy cập gần nhất (cho nút "Tiếp tục học")
    [HttpGet("me/latest")]
    public async Task<IActionResult> GetLatest()
    {
        var userId = GetUserId();
        var data = await _service.GetLatestAsync(userId);
        return Ok(new { success = true, data });
    }
}