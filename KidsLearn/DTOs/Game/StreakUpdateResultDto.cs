namespace KidsLearn.DTOs.Game;

// Kết quả sau khi cập nhật Streak khi user làm quiz
public class StreakUpdateResultDto
{
    public int CurrentStreak { get; set; }     // Chuỗi hiện tại (sau khi update)
    public int LongestStreak { get; set; }     // Kỷ lục
    public bool IsNewRecord { get; set; }      // Có lập kỷ lục mới không
    public bool IsMilestone { get; set; }      // Có đạt mốc 3/7/14/30/100 không
    public string? MilestoneMessage { get; set; } // Lời chúc mừng nếu đạt mốc
}