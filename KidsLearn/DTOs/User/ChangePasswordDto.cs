using System.ComponentModel.DataAnnotations;

namespace KidsLearn.DTOs.User;

public class ChangePasswordDto
{
    [Required(ErrorMessage = "Mật khẩu cũ không được để trống")]
    public string OldPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mật khẩu mới không được để trống")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu mới phải có ít nhất 6 ký tự")]
    public string NewPassword { get; set; } = string.Empty;
}