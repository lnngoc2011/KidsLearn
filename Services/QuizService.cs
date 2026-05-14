using KidsLearn.Data;
using KidsLearn.DTOs.Quiz;
using KidsLearn.Models;
using KidsLearn.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KidsLearn.Services;

public class QuizService : IQuizService
{
    private readonly KidsLearnDbContext _context;

    public QuizService(KidsLearnDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Lấy danh sách Quiz của Unit (KHÔNG trả về IsCorrect → chống cheat)
    /// ✨ FIX: _context.Quiz → _context.Quizzes, q.Answer → q.Answers
    /// </summary>
    public async Task<List<QuizDto>> GetByUnitAsync(int unitId)
    {
        return await _context.Quizzes
            .Where(q => q.UnitId == unitId)
            .Include(q => q.Answers)
            .Select(q => new QuizDto
            {
                QuizId = q.QuizId,
                QuestionText = q.QuestionText ?? "",
                QuestionType = q.QuestionType ?? "text",
                ImageUrl = q.ImageUrl,
                TtsText = q.TtsText,
                Answers = q.Answers.Select(a => new AnswerDto
                {
                    AnswerId = a.AnswerId,
                    AnswerText = a.AnswerText ?? "",
                    ImageUrl = a.ImageUrl
                    // KHÔNG trả về IsCorrect
                }).ToList()
            })
            .ToListAsync();
    }

    /// <summary>
    /// Học sinh nộp bài Quiz
    /// ✨ MỞ RỘNG: Sau khi chấm sẽ:
    ///   1. Lưu LearningProgress (1 record cho lượt làm bài)
    ///   2. Lưu QuizAttemptDetail (N records cho từng câu)
    ///   3. Tính số sao, lời khen
    /// </summary>
    public async Task<QuizResultDto> SubmitQuizAsync(int userId, SubmitQuizRequestDto request)
    {
        // 1. Lấy tất cả Quiz của Unit
        var quizzes = await _context.Quizzes
            .Where(q => q.UnitId == request.UnitId)
            .Include(q => q.Answers)
            .ToListAsync();

        if (!quizzes.Any())
            throw new KeyNotFoundException("Không tìm thấy câu hỏi cho Unit này!");

        // 2. Chấm điểm từng câu
        var details = new List<QuizAnswerDetailDto>();
        var attemptDetails = new List<QuizAttemptDetail>(); // để lưu DB
        int correctCount = 0;

        foreach (var quiz in quizzes)
        {
            var correctAnswer = quiz.Answers.FirstOrDefault(a => a.IsCorrect);
            if (correctAnswer == null) continue;

            // Học sinh có thể không chọn câu nào (TryGetValue trả false)
            bool hasSelected = request.Answers.TryGetValue(quiz.QuizId, out int selectedAnswerId);
            bool isCorrect = hasSelected && selectedAnswerId == correctAnswer.AnswerId;

            if (isCorrect) correctCount++;

            // Map cho response trả về frontend
            details.Add(new QuizAnswerDetailDto
            {
                QuizId = quiz.QuizId,
                QuestionText = quiz.QuestionText ?? "",
                SelectedAnswerId = hasSelected ? selectedAnswerId : null,
                CorrectAnswerId = correctAnswer.AnswerId,
                IsCorrect = isCorrect
            });

            // Chuẩn bị record để lưu DB (Review Mode)
            attemptDetails.Add(new QuizAttemptDetail
            {
                QuizId = quiz.QuizId,
                SelectedAnswerId = hasSelected ? selectedAnswerId : null,
                IsCorrect = isCorrect
            });
        }

        // 3. Tính điểm (decimal cho đúng kiểu DB)
        decimal score = quizzes.Count > 0
            ? Math.Round((decimal)correctCount / quizzes.Count * 100, 2)
            : 0;

        // 4. ✨ Lưu LearningProgress (1 record mới mỗi lượt làm bài)
        var progress = new LearningProgress
        {
            UserId = userId,
            UnitId = request.UnitId,
            Score = score,
            CompletedAt = DateTime.UtcNow
        };
        _context.LearningProgresses.Add(progress);
        await _context.SaveChangesAsync(); // Save để có ProgressId

        // 5. ✨ Lưu chi tiết từng câu (QuizAttemptDetail) - cho Review Mode
        foreach (var detail in attemptDetails)
            detail.ProgressId = progress.ProgressId;

        _context.QuizAttemptDetails.AddRange(attemptDetails);
        await _context.SaveChangesAsync();

        // 6. ✨ Tính số sao và lời khen
        int stars = CalculateStars(score);
        string message = GetMotivationalMessage(score);

        return new QuizResultDto
        {
            ProgressId = progress.ProgressId,
            TotalQuestions = quizzes.Count,
            CorrectAnswers = correctCount,
            WrongAnswers = quizzes.Count - correctCount,
            Score = score,
            StarCount = stars,
            MotivationalMessage = message,
            Details = details
        };
    }

    /// <summary>
    /// ✨ MỚI: Lấy lại chi tiết 1 lượt làm bài (Review Mode)
    /// Kiểm tra UserId để học sinh chỉ xem được bài của mình
    /// </summary>
    public async Task<QuizResultDto?> GetReviewAsync(int userId, int progressId)
    {
        var progress = await _context.LearningProgresses
            .Include(p => p.QuizAttemptDetails)
                .ThenInclude(d => d.Quiz)
                    .ThenInclude(q => q.Answers)
            .FirstOrDefaultAsync(p => p.ProgressId == progressId && p.UserId == userId);

        if (progress == null) return null;

        var details = progress.QuizAttemptDetails.Select(d =>
        {
            var correctAnswer = d.Quiz.Answers.FirstOrDefault(a => a.IsCorrect);
            return new QuizAnswerDetailDto
            {
                QuizId = d.QuizId,
                QuestionText = d.Quiz.QuestionText ?? "",
                SelectedAnswerId = d.SelectedAnswerId,
                CorrectAnswerId = correctAnswer?.AnswerId ?? 0,
                IsCorrect = d.IsCorrect
            };
        }).ToList();

        int correctCount = details.Count(d => d.IsCorrect);
        int stars = CalculateStars(progress.Score);
        string message = GetMotivationalMessage(progress.Score);

        return new QuizResultDto
        {
            ProgressId = progress.ProgressId,
            TotalQuestions = details.Count,
            CorrectAnswers = correctCount,
            WrongAnswers = details.Count - correctCount,
            Score = progress.Score,
            StarCount = stars,
            MotivationalMessage = message,
            Details = details
        };
    }

    /// <summary>
    /// Admin tạo câu hỏi mới (kèm đáp án)
    /// ✨ FIX: _context.Quiz → Quizzes, AudioUrl → ImageUrl + TtsText
    /// </summary>
    public async Task<QuizDto> CreateQuizAsync(CreateUpdateQuizDto dto)
    {
        // Phải có ít nhất 1 đáp án đúng
        if (!dto.Answers.Any(a => a.IsCorrect))
            throw new InvalidOperationException("Quiz phải có ít nhất 1 đáp án đúng!");

        var quiz = new Quiz
        {
            UnitId = dto.UnitId,
            QuestionText = dto.QuestionText,
            QuestionType = dto.QuestionType,
            ImageUrl = dto.ImageUrl,
            TtsText = dto.TtsText,
            Answers = dto.Answers.Select(a => new Answer
            {
                AnswerText = a.AnswerText,
                ImageUrl = a.ImageUrl,
                IsCorrect = a.IsCorrect
            }).ToList()
        };

        _context.Quizzes.Add(quiz);
        await _context.SaveChangesAsync();

        return MapToQuizDto(quiz);
    }

    /// <summary>
    /// Admin sửa câu hỏi
    /// </summary>
    public async Task<QuizDto?> UpdateQuizAsync(int quizId, CreateUpdateQuizDto dto)
    {
        var quiz = await _context.Quizzes
            .Include(q => q.Answers)
            .FirstOrDefaultAsync(q => q.QuizId == quizId);

        if (quiz == null) return null;

        if (!dto.Answers.Any(a => a.IsCorrect))
            throw new InvalidOperationException("Quiz phải có ít nhất 1 đáp án đúng!");

        quiz.QuestionText = dto.QuestionText;
        quiz.QuestionType = dto.QuestionType;
        quiz.ImageUrl = dto.ImageUrl;
        quiz.TtsText = dto.TtsText;

        // Xóa đáp án cũ, thêm đáp án mới
        // ✨ FIX: _context.Answer → _context.Answers
        _context.Answers.RemoveRange(quiz.Answers);

        quiz.Answers = dto.Answers.Select(a => new Answer
        {
            AnswerText = a.AnswerText,
            ImageUrl = a.ImageUrl,
            IsCorrect = a.IsCorrect
        }).ToList();

        await _context.SaveChangesAsync();
        return MapToQuizDto(quiz);
    }

    /// <summary>
    /// Admin xóa câu hỏi (CASCADE sẽ xóa Answer luôn)
    /// </summary>
    public async Task<bool> DeleteQuizAsync(int quizId)
    {
        var quiz = await _context.Quizzes.FindAsync(quizId);
        if (quiz == null) return false;

        _context.Quizzes.Remove(quiz);
        await _context.SaveChangesAsync();
        return true;
    }

    // =====================================================
    // HELPER METHODS
    // =====================================================

    // ✨ MỚI: Tính số sao theo điểm
    private static int CalculateStars(decimal score)
    {
        if (score >= 90) return 3;
        if (score >= 70) return 2;
        if (score >= 50) return 1;
        return 0;
    }

    // ✨ MỚI: Lời khen theo điểm
    private static string GetMotivationalMessage(decimal score)
    {
        if (score >= 90) return "Tuyệt vời! Con là thiên tài tiếng Anh! 🏆";
        if (score >= 70) return "Giỏi lắm! Con làm rất tốt rồi! 🌟";
        if (score >= 50) return "Cố lên nào! Con sắp giỏi rồi! 💪";
        return "Đừng buồn nhé! Hãy thử lại lần nữa! 🌈";
    }

    // Map Quiz Model → DTO
    private static QuizDto MapToQuizDto(Quiz quiz) => new()
    {
        QuizId = quiz.QuizId,
        QuestionText = quiz.QuestionText ?? "",
        QuestionType = quiz.QuestionType ?? "text",
        ImageUrl = quiz.ImageUrl,
        TtsText = quiz.TtsText,
        Answers = quiz.Answers.Select(a => new AnswerDto
        {
            AnswerId = a.AnswerId,
            AnswerText = a.AnswerText ?? "",
            ImageUrl = a.ImageUrl
        }).ToList()
    };
}