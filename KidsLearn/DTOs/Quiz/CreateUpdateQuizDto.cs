using System.ComponentModel.DataAnnotations;

namespace KidsLearn.DTOs.Quiz;

public class CreateUpdateQuizDto
{
    [Required]
    public int UnitId { get; set; }

    // Quiz dạng "image" hoặc "audio" có thể không cần text
    // Validate sẽ làm ở Service: phải có ít nhất 1 trong (text/image/tts)
    [StringLength(255)]
    public string? QuestionText { get; set; }

    // "text" | "image" | "audio"
    [Required]
    [RegularExpression("^(text|image|audio)$",
        ErrorMessage = "QuestionType phải là: text, image hoặc audio")]
    public string QuestionType { get; set; } = "text";

    public IFormFile? ImageFile { get; set; }

    [StringLength(255)]
    public string? TtsText { get; set; }

    // Admin tạo câu hỏi kèm đáp án luôn trong 1 request
    [Required]
    [MinLength(2, ErrorMessage = "Phải có ít nhất 2 đáp án")]
    public List<CreateAnswerDto> Answers { get; set; } = new();
}

public class CreateAnswerDto
{
    // đáp án dạng "image" có thể không cần text
    // Validate sẽ làm ở Service: phải có ít nhất 1 trong (text/image)
    [StringLength(255)]
    public string? AnswerText { get; set; }

    // RegularExpression để match với CHECK constraint
    [RegularExpression("^(text|image)$",
        ErrorMessage = "AnswerType phải là: text hoặc image")]
    public string AnswerType { get; set; } = "text";

    public IFormFile? ImageFile { get; set; }

    public bool IsCorrect { get; set; }
}