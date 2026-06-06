using System.ComponentModel.DataAnnotations;

namespace KidsLearn.DTOs.Auth;

// Dữ liệu Frontend gửi lên khi đăng ký
public class RegisterRequestDto
{
    [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
    [StringLength(50, MinimumLength = 3,
        ErrorMessage = "Tên đăng nhập phải từ 3-50 ký tự")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mật khẩu không được để trống")]
    [StringLength(100, MinimumLength = 6,
        ErrorMessage = "Mật khẩu phải từ 6-100 ký tự")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu")]
    [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
    public string ConfirmPassword { get; set; } = string.Empty;
}