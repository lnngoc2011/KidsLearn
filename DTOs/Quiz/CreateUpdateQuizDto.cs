using System.ComponentModel.DataAnnotations;

namespace KidsLearn.DTOs.Quiz;

public class CreateUpdateQuizDto
{
    [Required]
    public int UnitId { get; set; }

    [Required(ErrorMessage = "Câu hỏi không được để trống")]
    [StringLength(255)]
    public string QuestionText { get; set; } = string.Empty;

    // "text" | "image" | "audio"
    [Required]
    [RegularExpression("^(text|image|audio)$",
        ErrorMessage = "QuestionType phải là: text, image hoặc audio")]
    public string QuestionType { get; set; } = "text";

    // ✨ FIX: Đổi AudioUrl → ImageUrl + TtsText
    [StringLength(255)]
    public IFormFile? ImageFile { get; set; }

    [StringLength(255)]
    public string? TtsText { get; set; }

    // Admin tạo câu hỏi kèm đáp án luôn trong 1 request
    // ✨ FIX: Đổi "Answer" → "Answers" (số nhiều)
    [Required]
    [MinLength(2, ErrorMessage = "Phải có ít nhất 2 đáp án")]
    public List<CreateAnswerDto> Answers { get; set; } = new();
}

public class CreateAnswerDto
{
    [Required]
    [StringLength(255)]
    public string AnswerText { get; set; } = string.Empty;

    [StringLength(255)]
    public IFormFile? ImageFile { get; set; }

    public bool IsCorrect { get; set; }
}
public class CreateUpdateImageQuizDto

{
    [Required]
    public int UnitId { get; set; }

    [Required(ErrorMessage = "Câu hỏi không được để trống")]
    [StringLength(255)]
    public string QuestionText { get; set; } = string.Empty;

    // "text" | "image" | "audio"
    [Required]
    [RegularExpression("^(text|image|audio)$",
        ErrorMessage = "QuestionType phải là: text, image hoặc audio")]
    public string QuestionType { get; set; } = "text";

    // ✨ FIX: Đổi AudioUrl → ImageUrl + TtsText
    [StringLength(255)]
    public IFormFile? ImageFile { get; set; }

    [StringLength(255)]
    public string? TtsText { get; set; }

    // Admin tạo câu hỏi kèm đáp án luôn trong 1 request
    // ✨ FIX: Đổi "Answer" → "Answers" (số nhiều)
    [Required]
    [MinLength(2, ErrorMessage = "Phải có ít nhất 2 đáp án")]
    public List<CreateAnswerDto> Answers { get; set; } = new();
}