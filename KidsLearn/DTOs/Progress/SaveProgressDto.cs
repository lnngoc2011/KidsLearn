using System.ComponentModel.DataAnnotations;

namespace KidsLearn.DTOs.Progress;

// Dữ liệu gửi lên để lưu tiến độ học tập
// UserId lấy từ JWT — không để Frontend tự gửi UserId của người khác
public class SaveProgressDto
{
    [Required]
    public int UnitId { get; set; }

    [Required]
    [Range(0, 100, ErrorMessage = "Điểm phải từ 0 đến 100")]
    public decimal Score { get; set; }
}