using System.ComponentModel.DataAnnotations;

namespace KidsLearn.DTOs.Quiz;

// Dữ liệu học sinh gửi lên khi nộp bài
// Gửi 1 lần toàn bộ bài — không gửi từng câu
public class SubmitQuizRequestDto
{
    [Required]
    public int UnitId { get; set; }

    // Key: QuizId — Value: AnswerId học sinh chọn
    // Dictionary giúp map nhanh câu hỏi nào → đáp án nào
    // ✨ FIX: Đổi "Answer" → "Answers"
    [Required]
    public Dictionary<int, int> Answers { get; set; } = new();
}