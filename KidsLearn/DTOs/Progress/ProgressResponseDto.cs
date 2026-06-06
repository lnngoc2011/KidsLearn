namespace KidsLearn.DTOs.Progress;

// Thông tin 1 lần làm quiz hoàn thành
// ✨ FIX: Score là decimal đúng kiểu DB
public class ProgressResponseDto
{
    public int ProgressId { get; set; }
    public int UnitId { get; set; }
    public string UnitTitle { get; set; } = string.Empty;
    public decimal Score { get; set; }
    public DateTime CompletedAt { get; set; }
}