using System.ComponentModel.DataAnnotations;

namespace KidsLearn.DTOs.Auth;

// Dữ liệu Frontend gửi lên khi đăng nhập
public class LoginRequestDto
{
    [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mật khẩu không được để trống")]
    public string Password { get; set; } = string.Empty;
}