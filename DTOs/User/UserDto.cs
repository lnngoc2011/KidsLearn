namespace KidsLearn.DTOs.User;

public class UserDto
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public string? AvatarUrl { get; set; }
    public string? Role { get; set; }
    public DateTime? CreatedAt { get; set; }

    // ✨ MỚI: Thông tin Gamification
    public int TotalXP { get; set; }
    public int Level { get; set; }
}