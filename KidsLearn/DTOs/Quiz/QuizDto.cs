namespace KidsLearn.DTOs.Quiz;

// Thông tin 1 câu hỏi trắc nghiệm kèm danh sách đáp án

public class QuizDto
{
    public int QuizId { get; set; }
    public string QuestionText { get; set; } = string.Empty;

    public string QuestionType { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }   
    public string? TtsText { get; set; }     

    public List<AnswerDto> Answers { get; set; } = new();
}