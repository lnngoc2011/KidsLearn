using KidsLearn.DTOs.Quiz;
using KidsLearn.Models;

namespace KidsLearn.Services.Interfaces;

public interface IQuizService
{
    Task<List<QuizDto>> GetByUnitAsync(int unitId);

    Task<QuizResultDto> SubmitQuizAsync(int userId, SubmitQuizRequestDto request);

    Task<QuizResultDto?> GetReviewAsync(int userId, int progressId);
	Task<List<QuizDto>> GetMidReviewAsync(int gradeId, int reviewNumber);

	Task<List<QuizDto>> GetFinalReviewAsync(int gradeId);
	Task<QuizResultDto> SubmitReviewAsync(int userId, Dictionary<int, int> answers);
	// Admin
	Task<QuizDto> CreateQuizAsync(CreateUpdateQuizDto dto);
    Task<QuizDto?> UpdateQuizAsync(int quizId, CreateUpdateQuizDto dto);
    Task<bool> DeleteQuizAsync(int quizId);

}