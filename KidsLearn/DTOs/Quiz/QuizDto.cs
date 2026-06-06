namespace KidsLearn.DTOs.Quiz;

// Thông tin 1 câu hỏi trắc nghiệm kèm danh sách đáp án
// QuestionType: "text" | "image" | "audio"
// ✨ FIX: Đổi AudioUrl → ImageUrl + TtsText cho khớp Model
public class QuizDto
{
    public int QuizId { get; set; }
    public string QuestionText { get; set; } = string.Empty;

    public string QuestionType { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }    // Hình minh họa câu hỏi (cho dạng "image")
    public string? TtsText { get; set; }     // Văn bản TTS (cho dạng "audio")

    // ✨ FIX: Đổi "Answer" → "Answers" (số nhiều)
    public List<AnswerDto> Answers { get; set; } = new();
}