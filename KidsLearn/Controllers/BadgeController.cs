using System.Security.Claims;
using KidsLearn.Common;
using KidsLearn.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KidsLearn.Controllers;

/// <summary>
/// Dùng để xử lý các yêu cầu liên quan đến huy hiệu (badges) của người dùng
/// </summary>
[ApiController]
[Route("api/badges")]
[Authorize]
public class BadgeController : ControllerBase
{
    private readonly IGameService _gamification;

    public BadgeController(IGameService gamification)
    {
        _gamification = gamification;
    }

    /// <summary>
    /// Lấy toàn bộ huy hiệu trong hệ thống kèm trạng thái đạt được
    /// GET /api/badges
    /// Trả về cả huy hiệu chưa đạt (IsEarned = false) để hiển thị "khoá"
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllWithStatus()
    {
        var userId = GetUserId();
        if (userId == 0)
            return Unauthorized(ApiResponse<string>.Fail("Phiên đăng nhập hết hạn"));

        var badges = await _gamification.GetAllBadgesWithStatusAsync(userId);
        return Ok(ApiResponse<object>.Ok(badges));
    }

    /// <summary>
    /// Chỉ lấy huy hiệu user đã đạt
    /// GET /api/badges/me
    /// </summary>
    [HttpGet("me")]
    public async Task<IActionResult> GetMyEarnedBadges()
    {
        var userId = GetUserId();
        if (userId == 0)
            return Unauthorized(ApiResponse<string>.Fail("Phiên đăng nhập hết hạn"));

        var badges = await _gamification.GetEarnedBadgesAsync(userId);
        return Ok(ApiResponse<object>.Ok(badges));
    }

    /// <summary>
    /// Lấy danh sách huy hiệu CHƯA đạt (locked - hiển thị "???")
    /// GET /api/badges/me/locked
    /// </summary>
    [HttpGet("me/locked")]
    public async Task<IActionResult> GetMyLockedBadges()
    {
        var userId = GetUserId();
        if (userId == 0)
            return Unauthorized(ApiResponse<string>.Fail("Phiên đăng nhập hết hạn"));

        var all = await _gamification.GetAllBadgesWithStatusAsync(userId);
        var locked = all.Where(b => !b.IsEarned).ToList();
        return Ok(ApiResponse<object>.Ok(locked));
    }
    /// <summary>
    /// Dùng để lấy UserId từ claim trong JWT token
    /// </summary>
    /// <returns></returns>
    private int GetUserId()
    {
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        return idClaim != null && int.TryParse(idClaim.Value, out int id) ? id : 0;
    }
}