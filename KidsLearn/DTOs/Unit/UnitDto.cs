namespace KidsLearn.DTOs.Unit;

// Thông tin cơ bản của 1 Unit — dùng trong danh sách
// Khi học sinh xem danh sách Unit của 1 Grade
public class UnitDto
{
    public int UnitId { get; set; }
    public int GradeId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public int OrderIndex { get; set; }

    // Thống kê hiển thị trên card Unit
    public int VocabCount { get; set; }  // Số từ vựng
    public int QuizCount { get; set; }   // Số câu hỏi
}