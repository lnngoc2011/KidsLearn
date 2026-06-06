using System.Security.Claims;
using KidsLearn.Common;
using KidsLearn.DTOs.Progress;
using KidsLearn.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KidsLearn.Controllers;

[ApiController]
[Route("api/progress")]
[Authorize]
/// Các endpoint liên quan đến tiến độ làm bài của học sinh (progress) 
/// ví dụ: Lưu tiến độ, Lấy lịch sử làm bài, Điểm cao nhất trên 1 Unit
public class ProgressController : ControllerBase
{
    private readonly IProgressService _progressService;

    public ProgressController(IProgressService progressService)
    {
        _progressService = progressService;
    }

    /// <summary>
    /// Lưu tiến độ (thường ít dùng vì Quiz Submit đã tự lưu)
    /// POST /api/progress
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Save([FromBody] SaveProgressDto dto)
    {
        var userId = GetUserId();
        if (userId == 0)
            return Unauthorized(ApiResponse<string>.Fail("Phiên đăng nhập hết hạn"));

        try
        {
            var result = await _progressService.SaveProgressAsync(userId, dto);
            return Ok(ApiResponse<object>.Ok(result, "Lưu tiến độ thành công"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<string>.Fail(ex.Message));
        }
    }

    /// <summary>
    /// Lấy lịch sử làm bài của user hiện tại
    /// GET /api/progress/me
    /// </summary>
    [HttpGet("me")]
    public async Task<IActionResult> GetMyProgress()
    {
        var userId = GetUserId();
        if (userId == 0)
            return Unauthorized(ApiResponse<string>.Fail("Phiên đăng nhập hết hạn"));

        var list = await _progressService.GetByUserAsync(userId);
        return Ok(ApiResponse<object>.Ok(list));
    }

    /// <summary>
    /// ✨ MỚI: Điểm cao nhất của user trên 1 Unit
    /// GET /api/progress/me/best?unitId=5
    /// </summary>
    [HttpGet("me/best")]
    public async Task<IActionResult> GetMyBest([FromQuery] int unitId)
    {
        var userId = GetUserId();
        if (userId == 0)
            return Unauthorized(ApiResponse<string>.Fail("Phiên đăng nhập hết hạn"));

        var best = await _progressService.GetBestScoreAsync(userId, unitId);
        if (best == null)
            return Ok(ApiResponse<object>.Ok( "Chưa làm bài Unit này"));
        return Ok(ApiResponse<object>.Ok(best));
    }

    // Helper method để lấy UserId từ JWT Token
    private int GetUserId()
    {
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        return idClaim != null && int.TryParse(idClaim.Value, out int id) ? id : 0;
    }
}