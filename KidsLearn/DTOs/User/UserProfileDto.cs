namespace KidsLearn.DTOs.User;

// ✨ MỚI: Profile tổng hợp của học sinh trên Dashboard cá nhân
// Gộp dữ liệu từ nhiều bảng: User, UserStreak, LearningProgress
public class UserProfileDto
{
    // Thông tin cơ bản
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public string? AvatarUrl { get; set; }

    // Game
    public int TotalXP { get; set; }
    public int Level { get; set; }
    public string LevelName { get; set; } = string.Empty;  // Beginner/Explorer/Smart Kid/Vocab Master

    // Streak
    public int CurrentStreak { get; set; }
    public int LongestStreak { get; set; }

    // Thống kê học tập
    public int TotalUnitsAttempted { get; set; }   // Số Unit đã thử
    public int TotalUnitsCompleted { get; set; }   // Số Unit đã hoàn thành (>= 70 điểm)
    public decimal AverageScore { get; set; }      // Điểm trung bình (max mỗi Unit)
    public int TotalQuizzesTaken { get; set; }     // Tổng số lượt làm quiz

    // Huy hiệu
    public int BadgesEarned { get; set; }          // Số huy hiệu đã đạt
}