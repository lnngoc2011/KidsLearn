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

    // ===== ADMIN =====

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(ApiResponse<object>.Ok(users));
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
            return NotFound(ApiResponse<string>.Fail("Không tìm thấy user"));
        return Ok(ApiResponse<object>.Ok(user));
    }

    [HttpPut("{id}/role")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ChangeRole(int id, [FromBody] ChangeRoleDto dto)
    {
        var updated = await _userService.ChangeRoleAsync(id, dto);
        if (updated == null)
            return NotFound(ApiResponse<string>.Fail("Không tìm thấy user"));
        return Ok(ApiResponse<object>.Ok(updated, "Đã đổi vai trò"));
    }

    [HttpPut("{id}/reset-password")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ResetPassword(int id, [FromBody] ResetPasswordDto dto)
    {
        var ok = await _userService.ResetPasswordAsync(id, dto.NewPassword);
        if (!ok) return NotFound(ApiResponse<string>.Fail("Không tìm thấy user"));
        return Ok(ApiResponse<string>.Ok("OK", "Đã đặt lại mật khẩu"));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var requesterId = GetUserId();
        if (requesterId == id)
            return BadRequest(ApiResponse<string>.Fail("Không thể xóa chính tài khoản đang đăng nhập"));

        var ok = await _userService.DeleteUserAsync(id);
        if (!ok) return NotFound(ApiResponse<string>.Fail("Không tìm thấy user"));
        return Ok(ApiResponse<string>.Ok("OK", "Đã xóa user"));
    }

    private int GetUserId()
    {
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        return idClaim != null && int.TryParse(idClaim.Value, out int id) ? id : 0;
    }
}

// DTO simple cho reset password
public class ResetPasswordDto
{
    public string NewPassword { get; set; } = "Kid@123";
}