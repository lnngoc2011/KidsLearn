using KidsLearn.DTOs.Vocabulary;
using KidsLearn.DTOs.Quiz;

namespace KidsLearn.DTOs.Unit;

// Unit kèm DANH SÁCH TỪ VỰNG VÀ QUIZ bên trong
// Dùng khi học sinh click vào 1 Unit để bắt đầu học
public class UnitDetailDto
{
    public int UnitId { get; set; }
    public int GradeId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public int OrderIndex { get; set; }

    // Danh sách từ vựng và quiz thuộc Unit này
    public List<VocabularyDto> Vocabularies { get; set; } = new();
    public List<QuizDto> Quizzes { get; set; } = new();
}