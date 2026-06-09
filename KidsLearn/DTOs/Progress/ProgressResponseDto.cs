namespace KidsLearn.DTOs.Progress;

// Thông tin 1 lần làm quiz hoàn thành
public class ProgressResponseDto
{
    public int ProgressId { get; set; }
    public int UnitId { get; set; }
    public string UnitTitle { get; set; } = string.Empty;
    public decimal Score { get; set; }
    public DateTime CompletedAt { get; set; }
}