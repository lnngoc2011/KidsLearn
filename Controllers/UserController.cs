using System.Security.Claims;
using KidsLearn.Common;
using KidsLearn.DTOs.User;
using KidsLearn.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KidsLearn.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// ✨ MỚI: Profile tổng hợp của user (Streak, XP, Level, thống kê)
    /// GET /api/users/me/profile
    /// </summary>
    [HttpGet("me/profile")]
    public async Task<IActionResult> GetMyProfile()
    {
        var userId = GetUserId();
        if (userId == 0)
            return Unauthorized(ApiResponse<string>.Fail("Phiên đăng nhập hết hạn"));

        var profile = await _userService.GetMyProfileAsync(userId);
        if (profile == null)
            return NotFound(ApiResponse<string>.Fail("Không tìm thấy user"));

        return Ok(ApiResponse<object>.Ok(profile));
    }

    /// <summary>
    /// User cập nhật profile (FullName, Avatar)
    /// PUT /api/users/me
    /// </summary>
    [HttpPut("me")]
    public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateProfileDto dto)
    {
        var userId = GetUserId();
        if (userId == 0)
            return Unauthorized(ApiResponse<string>.Fail("Phiên đăng nhập hết hạn"));

        var updated = await _userService.UpdateProfileAsync(userId, dto);
        if (updated == null)
            return NotFound(ApiResponse<string>.Fail("Không tìm thấy user"));

        return Ok(ApiResponse<object>.Ok(updated, "Cập nhật thành công"));
    }

    /// <summary>
    /// User đổi mật khẩu
    /// PUT /api/users/me/password
    /// </summary>
    [HttpPut("me/password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        var userId = GetUserId();
        if (userId == 0)
            return Unauthorized(ApiResponse<string>.Fail("Phiên đăng nhập hết hạn"));

        try
        {
            await _userService.ChangePasswordAsync(userId, dto);
            return Ok(ApiResponse<string>.Ok("OK", "Đổi mật khẩu thành công"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ApiResponse<string>.Fail(ex.Message));
        }
    }

    private int GetUserId()
    {
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        return idClaim != null && int.TryParse(idClaim.Value, out int id) ? id : 0;
    }
}