using System.ComponentModel.DataAnnotations;

namespace KidsLearn.DTOs.User;

// User cập nhật thông tin cá nhân (không gồm Role - chỉ Admin sửa được Role)
public class UpdateProfileDto
{
    [StringLength(100)]
    public string? FullName { get; set; }

}