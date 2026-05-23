namespace KidsLearn.DTOs.Progress;

// Thống kê tổng quan của 1 học sinh 
// Dùng cho các API quick-stat khi không cần full UserProfileDto
public class UserStatsDto
{
    // Tổng số Unit đã thử (làm quiz ít nhất 1 lần)
    public int AttemptedUnits { get; set; }

    // Tổng số Unit đã hoàn thành (đạt Score >= 70)
    public int CompletedUnits { get; set; }

    // Điểm trung bình (trung bình điểm cao nhất mỗi Unit)
    public decimal AverageScore { get; set; }

    // Tổng số lượt làm quiz
    public int TotalQuizAttempts { get; set; }
}