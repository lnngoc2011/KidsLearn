using System.ComponentModel.DataAnnotations;

namespace KidsLearn.DTOs.User;

// Admin sửa role của user (Student / Admin)
public class ChangeRoleDto
{
    [Required]
    [RegularExpression("^(Student|Admin)$", ErrorMessage = "Role phải là Student hoặc Admin")]
    public string Role { get; set; } = "Student";
}