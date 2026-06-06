namespace KidsLearn.DTOs.Game;

// Thông tin huy hiệu trả về frontend
public class BadgeDto
{
    public int BadgeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? IconUrl { get; set; }
    public string? ConditionType { get; set; }

    public bool IsEarned { get; set; }         // User đã đạt chưa
    public DateTime? EarnedAt { get; set; }    // Thời gian đạt (null nếu chưa)
}