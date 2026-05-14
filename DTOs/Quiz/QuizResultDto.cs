namespace KidsLearn.DTOs.Quiz;

// ✨ MỞ RỘNG: Kết quả sau khi chấm bài, kèm thông tin Gamification
public class QuizResultDto
{
    public int ProgressId { get; set; }              // ID lượt làm bài để frontend gọi review
    public int TotalQuestions { get; set; }          // Tổng số câu hỏi
    public int CorrectAnswers { get; set; }          // Số câu đúng
    public int WrongAnswers { get; set; }            // Số câu sai
    public decimal Score { get; set; }               // Điểm 0-100

    // ✨ MỚI: Gamification
    public int StarCount { get; set; }               // 0-3 sao
    public string MotivationalMessage { get; set; } = string.Empty; // Lời khen

    // Chi tiết từng câu để học sinh xem lại (Review Mode)
    public List<QuizAnswerDetailDto> Details { get; set; } = new();
}

// Chi tiết 1 câu hỏi trong kết quả
public class QuizAnswerDetailDto
{
    public int QuizId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public int? SelectedAnswerId { get; set; }  // Có thể null nếu học sinh không chọn
    public int CorrectAnswerId { get; set; }
    public bool IsCorrect { get; set; }
}