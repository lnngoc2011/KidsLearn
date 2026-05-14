using KidsLearn.Common;
using KidsLearn.DTOs.User;
using KidsLearn.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KidsLearn.Controllers.Admin;

[ApiController]
[Route("api/admin/users")]
[Authorize(Policy = "AdminOnly")]
public class AdminUserController : ControllerBase
{
    private readonly IUserService _userService;

    public AdminUserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(ApiResponse<object>.Ok(users));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
            return NotFound(ApiResponse<string>.Fail("Không tìm thấy user"));
        return Ok(ApiResponse<object>.Ok(user));
    }

    /// <summary>
    /// Đổi role user (Student / Admin)
    /// </summary>
    [HttpPut("{id}/role")]
    public async Task<IActionResult> ChangeRole(int id, [FromBody] ChangeRoleDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<string>.Fail("Dữ liệu không hợp lệ"));

        var result = await _userService.ChangeRoleAsync(id, dto);
        if (result == null)
            return NotFound(ApiResponse<string>.Fail("Không tìm thấy user"));
        return Ok(ApiResponse<object>.Ok(result, "Đổi role thành công"));
    }

    /// <summary>
    /// Reset password cho user (mặc định "123456")
    /// </summary>
    [HttpPost("{id}/reset-password")]
    public async Task<IActionResult> ResetPassword(int id)
    {
        var ok = await _userService.ResetPasswordAsync(id, "123456");
        if (!ok)
            return NotFound(ApiResponse<string>.Fail("Không tìm thấy user"));
        return Ok(ApiResponse<object>.Ok(
            new { newPassword = "123456" },
            "Reset mật khẩu thành công. Mật khẩu mới: 123456"));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _userService.DeleteUserAsync(id);
        if (!ok)
            return NotFound(ApiResponse<string>.Fail("Không tìm thấy user"));
        return Ok(ApiResponse<string>.Ok("OK", "Xóa user thành công"));
    }
}