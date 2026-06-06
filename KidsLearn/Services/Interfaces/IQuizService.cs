using KidsLearn.DTOs.Quiz;
using KidsLearn.Models;

namespace KidsLearn.Services.Interfaces;

public interface IQuizService
{
    // ✨ FIX: rename lessonId → unitId
    Task<List<QuizDto>> GetByUnitAsync(int unitId);

    // ✨ FIX: thêm userId để lưu LearningProgress
    Task<QuizResultDto> SubmitQuizAsync(int userId, SubmitQuizRequestDto request);

    // ✨ MỚI: Lấy chi tiết lượt làm bài để Review
    Task<QuizResultDto?> GetReviewAsync(int userId, int progressId);

    // Admin
    Task<QuizDto> CreateQuizAsync(CreateUpdateQuizDto dto);
    Task<QuizDto?> UpdateQuizAsync(int quizId, CreateUpdateQuizDto dto);
    Task<bool> DeleteQuizAsync(int quizId);

}