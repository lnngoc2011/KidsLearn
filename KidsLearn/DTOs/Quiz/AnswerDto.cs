namespace KidsLearn.DTOs.Quiz;

// Thông tin 1 đáp án của câu hỏi
// Lưu ý: KHÔNG trả về IsCorrect cho học sinh
// Nếu trả về IsCorrect → học sinh inspect response là biết đáp án ngay
public class AnswerDto
{
    public int AnswerId { get; set; }
    public string AnswerText { get; set; } = string.Empty;
    public string AnswerType { get; set; } = string.Empty; // "text" | "image" | "audio"
    public string? ImageUrl { get; set; }
}