using KidsLearn.Data;
using KidsLearn.DTOs.Quiz;
using KidsLearn.Models;
using KidsLearn.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KidsLearn.Services;

public class QuizService : IQuizService
{
    private readonly KidsLearnDbContext _context;
    private readonly IGameService _Game;  // ✨ MỚI: Inject GameService
    private readonly ICloudinaryService _cloudinaryService;

    private const decimal THRESHOLD_3_STARS = 90;
    private const decimal THRESHOLD_2_STARS = 70;
    private const decimal THRESHOLD_1_STAR = 50;

    // ✨ MỚI: Constants cho công thức XP
    private const int XP_BASE = 10;             // XP cơ bản khi nộp bài
    private const int XP_PER_CORRECT = 5;       // XP cho mỗi câu đúng
    private const int XP_BONUS_3_STARS = 20;    // Bonus khi đạt 3 sao

    public QuizService(KidsLearnDbContext context, IGameService Game, ICloudinaryService cloudinaryService)
    {
        _context = context;
        _Game = Game;
        _cloudinaryService = cloudinaryService;
    }

    public async Task<List<QuizDto>> GetByUnitAsync(int unitId)
    {
        return await _context.Quizzes
         .Where(q => q.UnitId == unitId)
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
             }).ToList()
         })
         .ToListAsync();
    }

    /// <summary>
    /// Học sinh nộp bài Quiz - ĐÃ TÍCH HỢP GAME
    /// Flow:
    ///   1. Chấm điểm
    ///   2. Lưu LearningProgress + QuizAttemptDetail
    ///   3. ✨ Cập nhật Streak
    ///   4. ✨ Cộng XP và check Level Up
    ///   5. ✨ Kiểm tra và trao huy hiệu mới
    /// </summary>
    public async Task<QuizResultDto> SubmitQuizAsync(int userId, SubmitQuizRequestDto request)
    {
        // ───── BƯỚC 1: Lấy Quiz của Unit ─────
        var quizzes = await _context.Quizzes
            .Where(q => q.UnitId == request.UnitId)
            .Include(q => q.Answers)
            .ToListAsync();

        if (!quizzes.Any())
            throw new KeyNotFoundException("Không tìm thấy câu hỏi cho Unit này!");

        // ───── BƯỚC 2: Chấm điểm ─────
        var details = new List<QuizAnswerDetailDto>();
        var attemptDetails = new List<QuizAttemptDetail>();
        int correctCount = 0;

        foreach (var quiz in quizzes)
        {
            var correctAnswer = quiz.Answers.FirstOrDefault(a => a.IsCorrect);
            if (correctAnswer == null) continue;

            bool hasSelected = request.Answers.TryGetValue(quiz.QuizId, out int selectedAnswerId);

            // Validate AnswerId thuộc Quiz này không (chống cheat)
            bool isValidAnswer = hasSelected && quiz.Answers.Any(a => a.AnswerId == selectedAnswerId);
            int? finalAnswerId = isValidAnswer ? selectedAnswerId : (int?)null;

            bool isCorrect = isValidAnswer && selectedAnswerId == correctAnswer.AnswerId;

            if (isCorrect) correctCount++;

            details.Add(new QuizAnswerDetailDto
            {
                QuizId = quiz.QuizId,
                QuestionText = quiz.QuestionText ?? "",
                SelectedAnswerId = finalAnswerId,
                CorrectAnswerId = correctAnswer.AnswerId,
                IsCorrect = isCorrect
            });

            attemptDetails.Add(new QuizAttemptDetail
            {
                QuizId = quiz.QuizId,
                SelectedAnswerId = finalAnswerId,
                IsCorrect = isCorrect
            });
        }

        decimal score = quizzes.Count > 0
            ? Math.Round((decimal)correctCount / quizzes.Count * 100, 2)
            : 0;

        // ───── BƯỚC 3: Lưu LearningProgress ─────
        var progress = new LearningProgress
        {
            UserId = userId,
            UnitId = request.UnitId,
            Score = score,
            CompletedAt = DateTime.UtcNow
        };
        _context.LearningProgresses.Add(progress);
        await _context.SaveChangesAsync();

        // ───── BƯỚC 4: Lưu QuizAttemptDetail ─────
        foreach (var detail in attemptDetails)
            detail.ProgressId = progress.ProgressId;

        _context.QuizAttemptDetails.AddRange(attemptDetails);
        await _context.SaveChangesAsync();

        // ───── BƯỚC 5: Tính sao + lời khen ─────
        int stars = CalculateStars(score);
        string message = GetMotivationalMessage(score);

        // ═══════════════════════════════════════════════
        // ✨ TÍCH HỢP GAMIFICATION ✨
        // ═══════════════════════════════════════════════

        // 5.1. Cập nhật Streak
        var streakResult = await _Game.UpdateStreakAsync(userId);

        // 5.2. Tính XP và cộng
        int xpToAdd = CalculateXP(correctCount, stars);
        var xpResult = await _Game.AddXPAsync(userId, xpToAdd);

        // 5.3. Kiểm tra trao huy hiệu mới
        var newBadges = await _Game.CheckAndAwardBadgesAsync(userId);

        // ───── BƯỚC 6: Trả kết quả đầy đủ ─────
        return new QuizResultDto
        {
            ProgressId = progress.ProgressId,
            TotalQuestions = quizzes.Count,
            CorrectAnswers = correctCount,
            WrongAnswers = quizzes.Count - correctCount,
            Score = score,
            StarCount = stars,
            MotivationalMessage = message,

            // Game info
            XPGained = xpResult.XPGained,
            TotalXP = xpResult.TotalXP,
            Level = xpResult.Level,
            LeveledUp = xpResult.LeveledUp,
            LevelName = xpResult.LevelName,
            CurrentStreak = streakResult.CurrentStreak,
            IsStreakMilestone = streakResult.IsMilestone,
            StreakMessage = streakResult.MilestoneMessage,
            NewBadges = newBadges,

            Details = details
        };
    }

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

        return new QuizResultDto
        {
            ProgressId = progress.ProgressId,
            TotalQuestions = details.Count,
            CorrectAnswers = correctCount,
            WrongAnswers = details.Count - correctCount,
            Score = progress.Score,
            StarCount = stars,
            MotivationalMessage = GetMotivationalMessage(progress.Score),
            Details = details
            // Review mode KHÔNG cộng XP/Streak nữa
        };
    }

    public async Task<QuizDto> CreateQuizAsync(CreateUpdateQuizDto dto)
    {
        if (!dto.Answers.Any(a => a.IsCorrect))
            throw new InvalidOperationException("Quiz phải có ít nhất 1 đáp án đúng!");

        // Upload ảnh cho Quiz (nullable)
        var imageQuizUrl = dto.ImageFile != null
            ? await _cloudinaryService.UploadImageAsync(dto.ImageFile)
            : null;

        var quiz = new Quiz
        {
            UnitId = dto.UnitId,
            QuestionText = dto.QuestionText,
            QuestionType = dto.QuestionType,
            ImageUrl = imageQuizUrl,        // 👈 ảnh của Quiz
            TtsText = dto.TtsText,
            Answers = new List<Answer>()
        };

        // Upload ảnh riêng cho từng Answer
        foreach (var a in dto.Answers)
        {
            var imageAnswerUrl = a.ImageFile != null
                ? await _cloudinaryService.UploadImageAsync(a.ImageFile)
                : null;

            quiz.Answers.Add(new Answer
            {
                AnswerText = a.AnswerText,
                ImageUrl = imageAnswerUrl,  // 👈 ảnh riêng của từng Answer
                IsCorrect = a.IsCorrect
            });
        }

        await _context.Quizzes.AddAsync(quiz);
        await _context.SaveChangesAsync();

        return MapToQuizDto(quiz);
    }

    public async Task<QuizDto?> UpdateQuizAsync(int quizId, CreateUpdateQuizDto dto)
    {
        var quiz = await _context.Quizzes
            .Include(q => q.Answers)
            .FirstOrDefaultAsync(q => q.QuizId == quizId);

        if (quiz == null) return null;

        if (!dto.Answers.Any(a => a.IsCorrect))
            throw new InvalidOperationException("Quiz phải có ít nhất 1 đáp án đúng!");

        // Xóa ảnh cũ của Quiz, upload ảnh mới nếu có
        if (dto.ImageFile != null)
        {
            if (!string.IsNullOrEmpty(quiz.ImageUrl))
            {
                var oldPublicId = _cloudinaryService.GetPublicIdFromUrl(quiz.ImageUrl);
                await _cloudinaryService.DeleteImageAsync(oldPublicId);
            }
            quiz.ImageUrl = await _cloudinaryService.UploadImageAsync(dto.ImageFile);
        }

        quiz.QuestionText = dto.QuestionText;
        quiz.QuestionType = dto.QuestionType;
        quiz.TtsText = dto.TtsText;

        // Xóa ảnh cũ của từng Answer trên Cloudinary
        foreach (var oldAnswer in quiz.Answers)
        {
            if (!string.IsNullOrEmpty(oldAnswer.ImageUrl))
            {
                var oldPublicId = _cloudinaryService.GetPublicIdFromUrl(oldAnswer.ImageUrl);
                await _cloudinaryService.DeleteImageAsync(oldPublicId);
            }
        }

        // Xóa Answer cũ trong DB
        _context.Answers.RemoveRange(quiz.Answers);

        // Tạo Answer mới với ảnh riêng từng cái
        var newAnswers = new List<Answer>();
        foreach (var a in dto.Answers)
        {
            var imageAnswerUrl = a.ImageFile != null
                ? await _cloudinaryService.UploadImageAsync(a.ImageFile)
                : null;

            newAnswers.Add(new Answer
            {
                AnswerText = a.AnswerText,
                ImageUrl = imageAnswerUrl,  // 👈 ảnh riêng của từng Answer
                IsCorrect = a.IsCorrect
            });
        }

        quiz.Answers = newAnswers;

        await _context.SaveChangesAsync();
        return MapToQuizDto(quiz);
    }

    public async Task<bool> DeleteQuizAsync(int quizId)
    {
        var quiz = await _context.Quizzes.FindAsync(quizId);

        if (quiz == null) return false;
        if (!string.IsNullOrEmpty(quiz.ImageUrl))
        {
            var oldPublicId = _cloudinaryService.GetPublicIdFromUrl(quiz.ImageUrl);
            await _cloudinaryService.DeleteImageAsync(oldPublicId);
        }
        _context.Quizzes.Remove(quiz);
        await _context.SaveChangesAsync();
        return true;
    }

    // ═══════════════════════════════════════════════
    // HELPER METHODS
    // ═══════════════════════════════════════════════

    private static int CalculateStars(decimal score)
    {
        if (score >= THRESHOLD_3_STARS) return 3;
        if (score >= THRESHOLD_2_STARS) return 2;
        if (score >= THRESHOLD_1_STAR) return 1;
        return 0;
    }

    private static string GetMotivationalMessage(decimal score)
    {
        if (score >= THRESHOLD_3_STARS) return "Tuyệt vời! Con là thiên tài tiếng Anh! 🏆";
        if (score >= THRESHOLD_2_STARS) return "Giỏi lắm! Con làm rất tốt rồi! 🌟";
        if (score >= THRESHOLD_1_STAR) return "Cố lên nào! Con sắp giỏi rồi! 💪";
        return "Đừng buồn nhé! Hãy thử lại lần nữa! 🌈";
    }

    // ✨ MỚI: Công thức tính XP
    // - Base: 10 XP cho việc nộp bài
    // - Bonus: +5 XP cho mỗi câu đúng
    // - Bonus: +20 XP nếu đạt 3 sao (Perfect-ish)
    private static int CalculateXP(int correctCount, int stars)
    {
        int xp = XP_BASE + correctCount * XP_PER_CORRECT;
        if (stars == 3) xp += XP_BONUS_3_STARS;
        return xp;
    }

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