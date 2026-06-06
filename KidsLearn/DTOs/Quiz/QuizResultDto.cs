using KidsLearn.DTOs.Game;

namespace KidsLearn.DTOs.Quiz;

// Kết quả sau khi nộp bài, kèm thông tin Game
public class QuizResultDto
{
	public int ProgressId { get; set; }
	public int TotalQuestions { get; set; }
	public int CorrectAnswers { get; set; }
	public int WrongAnswers { get; set; }
	public decimal Score { get; set; }

	// Game cơ bản
	public int StarCount { get; set; }
	public string MotivationalMessage { get; set; } = string.Empty;

	// ✨ MỚI: Thông tin Game mở rộng
	public int XPGained { get; set; }                // XP nhận được lần này
	public int TotalXP { get; set; }                 // Tổng XP hiện tại
	public int Level { get; set; }                   // Level sau khi tính lại
	public bool LeveledUp { get; set; }              // Có lên cấp không
	public string LevelName { get; set; } = string.Empty;

	// Streak
	public int CurrentStreak { get; set; }
	public bool IsStreakMilestone { get; set; }
	public string? StreakMessage { get; set; }

	// Huy hiệu MỚI đạt được trong lần này
	public List<BadgeDto> NewBadges { get; set; } = new();

	// Chi tiết từng câu để xem lại
	public List<QuizAnswerDetailDto> Details { get; set; } = new();
}

// Chi tiết 1 câu hỏi trong kết quả
public class QuizAnswerDetailDto
{
	public int QuizId { get; set; }
	public string QuestionText { get; set; } = string.Empty;
	public int? SelectedAnswerId { get; set; }
	public int CorrectAnswerId { get; set; }
	public bool IsCorrect { get; set; }
}