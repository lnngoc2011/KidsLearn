using KidsLearn.Common;
using KidsLearn.DTOs.User;
using KidsLearn.Services;
using KidsLearn.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KidsLearn.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
/// Các endpoint liên quan đến người dùng (user)
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    // ===== USER =====
    [HttpGet("me/profile")]
    [Authorize]
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

    // Endpoint này cho phép người dùng cập nhật thông tin cá nhân của mình, ví dụ: tên hiển thị (full name).
    [HttpPut("me")]
    [Authorize]
    public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateProfileDto dto)
    {
        var userId = GetUserId();
        if (userId == 0)
            return Unauthorized(ApiResponse<string>.Fail("Phiên đăng nhập hết hạn"));
        if (dto.FullName == null || dto.FullName.Length == 0)
            return BadRequest(ApiResponse<string>.Fail("Không có tên"));
        var updated = await _userService.UpdateProfileAsync(userId, dto.FullName);
        if (updated == null)
            return NotFound(ApiResponse<string>.Fail("Không tìm thấy user"));

        return Ok(ApiResponse<object>.Ok(updated, "Cập nhật thành công"));
    }

    // Endpoint này cho phép người dùng cập nhật ảnh đại diện (avatar) của mình.
    // Người dùng sẽ gửi một file ảnh, và hệ thống sẽ lưu trữ file đó và cập nhật đường dẫn ảnh đại diện trong hồ sơ người dùng.
    [HttpPut("me/avatar")]
    [Authorize]
    public async Task<IActionResult> UpdateMyAvatar(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(ApiResponse<string>.Fail("Không có file ảnh"));

        var userId = GetUserId();
        if (userId == 0)
            return Unauthorized(ApiResponse<string>.Fail("Phiên đăng nhập hết hạn"));

        var updated = await _userService.UpdateAvatarAsync(userId, file);
        if (updated == null)
            return NotFound(ApiResponse<string>.Fail("Không tìm thấy user"));

        return Ok(ApiResponse<object>.Ok(updated, "Cập nhật thành công"));
    }

    // Endpoint này cho phép người dùng đổi mật khẩu của mình. Người dùng cần cung cấp mật khẩu cũ để xác thực, và mật khẩu mới để cập nhật.
    [HttpPut("me/password")]
    [Authorize]
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
    // Endpoint này cho phép admin xem được danh sách tất cả người dùng (user) trong hệ thống.
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(ApiResponse<object>.Ok(users));
    }

    // Endpoint này cho phép admin xem được thông tin chi tiết của một người dùng cụ thể dựa trên ID của họ.
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
            return NotFound(ApiResponse<string>.Fail("Không tìm thấy user"));
        return Ok(ApiResponse<object>.Ok(user));
    }

    // Endpoint này cho phép admin thay đổi vai trò (role) của một người dùng cụ thể.
    // Admin có thể chuyển đổi giữa các vai trò như "User", "Teacher", "Admin".
    [HttpPut("{id}/role")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ChangeRole(int id, [FromBody] ChangeRoleDto dto)
    {
        var requesterId = GetUserId();
        if (requesterId == id)
            return BadRequest(ApiResponse<string>.Fail("Không thể đổi role của chính tài khoản"));
        var updated = await _userService.ChangeRoleAsync(id, dto);
        if (updated == null)
            return NotFound(ApiResponse<string>.Fail("Không tìm thấy user"));
        return Ok(ApiResponse<object>.Ok(updated, "Đã đổi vai trò"));
    }
    // Endpoint này cho phép admin đặt lại mật khẩu của một người dùng cụ thể về một giá trị mặc định (ví dụ: "Kid@123").

    [HttpPut("{id}/reset-password")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ResetPassword(int id, [FromBody] ResetPasswordDto dto)
    {
        var ok = await _userService.ResetPasswordAsync(id, dto.NewPassword);
        if (!ok) return NotFound(ApiResponse<string>.Fail("Không tìm thấy user"));
        return Ok(ApiResponse<string>.Ok("OK", "Đã đặt lại mật khẩu"));
    }

    // Endpoint này cho phép admin xóa một người dùng cụ thể khỏi hệ thống.
    // Việc xóa sẽ loại bỏ hoàn toàn hồ sơ và dữ liệu liên quan đến người dùng đó.
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
    // Hàm tiện ích để lấy ID người dùng từ claim trong token JWT
    private int GetUserId()
    {
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        return idClaim != null && int.TryParse(idClaim.Value, out int id) ? id : 0;
    }
}


// DTO dùng để nhận dữ liệu khi admin muốn đặt lại mật khẩu cho user. Mật khẩu mới sẽ được truyền vào trong trường NewPassword.
public class ResetPasswordDto
{
    public string NewPassword { get; set; } = "Kid@123";
}