using KidsLearn.Data;
using KidsLearn.DTOs.Game;
using KidsLearn.Models;
using KidsLearn.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KidsLearn.Services;

public class GameService : IGameService
{
    private readonly KidsLearnDbContext _context;

    // ===== HẰNG SỐ NGƯỠNG LEVEL =====
    // Level 1: 0-99 XP (Beginner)
    // Level 2: 100-499 XP (Explorer)
    // Level 3: 500-1999 XP (Smart Kid)
    // Level 4+: 2000+ XP (Vocab Master)
    private static readonly int[] LEVEL_THRESHOLDS = { 0, 100, 500, 2000 };

    // ===== CÁC MỐC STREAK ĐẶC BIỆT =====
    private static readonly int[] STREAK_MILESTONES = { 3, 7, 14, 30, 100 };

    public GameService(KidsLearnDbContext context)
    {
        _context = context;
    }

    // ═════════════════════════════════════════════════════
    // STREAK - CHUỖI NGÀY HỌC LIÊN TIẾP
    // ═════════════════════════════════════════════════════

    public async Task<StreakUpdateResultDto> UpdateStreakAsync(int userId)
    {
        var today = DateTime.UtcNow.Date;
        var yesterday = today.AddDays(-1);

        // Lấy streak hiện tại của user (nếu chưa có thì tạo mới)
        var streak = await _context.UserStreaks
            .FirstOrDefaultAsync(s => s.UserId == userId);

        if (streak == null)
        {
            // Lần đầu học → tạo record mới với streak = 1
            streak = new UserStreak
            {
                UserId = userId,
                CurrentStreak = 1,
                LongestStreak = 1,
                LastStudyDate = today
            };
            _context.UserStreaks.Add(streak);
        }
        else
        {
            var lastDate = streak.LastStudyDate?.Date;

            if (lastDate == today)
            {
                // Cùng ngày → không đổi gì cả
                // (User làm nhiều quiz trong 1 ngày chỉ tính 1 streak)
            }
            else if (lastDate == yesterday)
            {
                // Hôm qua → tăng streak +1
                streak.CurrentStreak++;
                streak.LastStudyDate = today;

                // Cập nhật kỷ lục nếu vượt
                if (streak.CurrentStreak > streak.LongestStreak)
                    streak.LongestStreak = streak.CurrentStreak;
            }
            else
            {
                // Nghỉ quá 1 ngày → reset về 1
                streak.CurrentStreak = 1;
                streak.LastStudyDate = today;
            }
        }

        await _context.SaveChangesAsync();

        // Kiểm tra có đạt mốc đặc biệt không
        bool isMilestone = STREAK_MILESTONES.Contains(streak.CurrentStreak);
        string? milestoneMessage = isMilestone
            ? GetMilestoneMessage(streak.CurrentStreak)
            : null;

        return new StreakUpdateResultDto
        {
            CurrentStreak = streak.CurrentStreak,
            LongestStreak = streak.LongestStreak,
            IsNewRecord = streak.CurrentStreak == streak.LongestStreak
                       && streak.CurrentStreak > 1,
            IsMilestone = isMilestone,
            MilestoneMessage = milestoneMessage
        };
    }

    // Lời chúc mừng theo mốc streak
    private static string GetMilestoneMessage(int streak) => streak switch
    {
        3 => "Tuyệt vời! Con đã học 3 ngày liên tiếp! 🎉",
        7 => "Xuất sắc! 1 tuần học liên tục! Con thật chăm chỉ! 🔥",
        14 => "Quá đỉnh! 2 tuần không nghỉ! 💪",
        30 => "Không thể tin được! 1 THÁNG học liên tục! 🏆",
        100 => "HUYỀN THOẠI! 100 ngày học liên tục! 👑",
        _ => "Tiếp tục phát huy nhé! ⭐"
    };

    // ═════════════════════════════════════════════════════
    // XP & LEVEL
    // ═════════════════════════════════════════════════════

    public async Task<XPUpdateResultDto> AddXPAsync(int userId, int xp)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            throw new KeyNotFoundException($"Không tìm thấy user ID = {userId}");

        int oldLevel = user.Level;
        user.TotalXP += xp;

        // Tính lại level dựa trên TotalXP mới
        int newLevel = CalculateLevel(user.TotalXP);
        user.Level = newLevel;

        await _context.SaveChangesAsync();

        return new XPUpdateResultDto
        {
            XPGained = xp,
            TotalXP = user.TotalXP,
            Level = newLevel,
            LeveledUp = newLevel > oldLevel,
            LevelName = GetLevelName(newLevel),
            XPToNextLevel = GetXPToNextLevel(user.TotalXP, newLevel)
        };
    }

    // Tính level từ tổng XP
    private static int CalculateLevel(int totalXP)
    {
        if (totalXP >= LEVEL_THRESHOLDS[3]) return 4; // Vocab Master
        if (totalXP >= LEVEL_THRESHOLDS[2]) return 3; // Smart Kid
        if (totalXP >= LEVEL_THRESHOLDS[1]) return 2; // Explorer
        return 1;                                      // Beginner
    }

    // Mapping Level → Tên (đồng bộ với UserService)
    private static string GetLevelName(int level) => level switch
    {
        1 => "Beginner",
        2 => "Explorer",
        3 => "Smart Kid",
        _ => "Vocab Master"
    };

    // XP còn cần để lên level kế tiếp
    private static int GetXPToNextLevel(int totalXP, int currentLevel)
    {
        if (currentLevel >= 4) return 0; // Đã max level
        int nextThreshold = LEVEL_THRESHOLDS[currentLevel]; // Index = currentLevel (vì array 0-based)
        return nextThreshold - totalXP;
    }

    // ═════════════════════════════════════════════════════
    // BADGE - HUY HIỆU
    // ═════════════════════════════════════════════════════

    public async Task<List<BadgeDto>> CheckAndAwardBadgesAsync(int userId)
    {
        // Lấy danh sách huy hiệu user CHƯA đạt
        var earnedBadgeIds = await _context.UserBadges
            .Where(ub => ub.UserId == userId)
            .Select(ub => ub.BadgeId)
            .ToListAsync();

        var unEarnedBadges = await _context.Badges
            .Where(b => !earnedBadgeIds.Contains(b.BadgeId))
            .ToListAsync();

        // Lấy số liệu thống kê của user (để check điều kiện)
        var stats = await GetUserStatsForBadgeCheck(userId);

        var newlyEarnedBadges = new List<BadgeDto>();

        // Duyệt từng huy hiệu chưa đạt, kiểm tra điều kiện
        foreach (var badge in unEarnedBadges)
        {
            bool conditionMet = CheckBadgeCondition(badge.ConditionType, stats);

            if (conditionMet)
            {
                // Trao huy hiệu - thêm vào bảng UserBadge
                var userBadge = new UserBadge
                {
                    UserId = userId,
                    BadgeId = badge.BadgeId,
                    EarnedAt = DateTime.UtcNow
                };
                _context.UserBadges.Add(userBadge);

                newlyEarnedBadges.Add(new BadgeDto
                {
                    BadgeId = badge.BadgeId,
                    Name = badge.Name,
                    Description = badge.Description,
                    IconUrl = badge.IconUrl,
                    ConditionType = badge.ConditionType,
                    IsEarned = true,
                    EarnedAt = userBadge.EarnedAt
                });
            }
        }

        if (newlyEarnedBadges.Any())
            await _context.SaveChangesAsync();

        return newlyEarnedBadges;
    }

    // Helper: lấy stats để kiểm tra điều kiện huy hiệu
    private async Task<BadgeCheckStats> GetUserStatsForBadgeCheck(int userId)
    {
        var totalQuizzes = await _context.LearningProgresses
            .CountAsync(p => p.UserId == userId);

        var perfectScores = await _context.LearningProgresses
            .CountAsync(p => p.UserId == userId && p.Score == 100);

        var completedUnits = await _context.LearningProgresses
            .Where(p => p.UserId == userId && p.Score >= 70)
            .Select(p => p.UnitId)
            .Distinct()
            .CountAsync();

        // Tổng từ vựng đã học = từ trong các Unit đã thử
        var attemptedUnitIds = await _context.LearningProgresses
            .Where(p => p.UserId == userId)
            .Select(p => p.UnitId)
            .Distinct()
            .ToListAsync();

        var totalWords = await _context.Vocabularies
            .CountAsync(v => attemptedUnitIds.Contains(v.UnitId));

        var streak = await _context.UserStreaks
            .FirstOrDefaultAsync(s => s.UserId == userId);

        // Tổng số sao đã đạt (mỗi quiz có 0-3 sao)
        var allScores = await _context.LearningProgresses
            .Where(p => p.UserId == userId)
            .Select(p => p.Score)
            .ToListAsync();

        int totalStars = allScores.Sum(s =>
        {
            if (s >= 90) return 3;
            if (s >= 70) return 2;
            if (s >= 50) return 1;
            return 0;
        });

        return new BadgeCheckStats
        {
            TotalQuizzes = totalQuizzes,
            PerfectScores = perfectScores,
            CompletedUnits = completedUnits,
            TotalWords = totalWords,
            CurrentStreak = streak?.CurrentStreak ?? 0,
            TotalStars = totalStars
        };
    }

    // Kiểm tra 1 huy hiệu có đủ điều kiện không
    private static bool CheckBadgeCondition(string? conditionType, BadgeCheckStats stats)
    {
        // Mapping ConditionType (lưu trong DB) → logic kiểm tra
        return conditionType switch
        {
            "COMPLETE_FIRST_QUIZ" => stats.TotalQuizzes >= 1,
            "STREAK_7" => stats.CurrentStreak >= 7,
            "STREAK_30" => stats.CurrentStreak >= 30,
            "LEARN_100_WORDS" => stats.TotalWords >= 100,
            "LEARN_500_WORDS" => stats.TotalWords >= 500,
            "QUIZ_SCORE_100" => stats.PerfectScores >= 1,
            "COMPLETE_10_UNITS" => stats.CompletedUnits >= 10,
            "COLLECT_50_STARS" => stats.TotalStars >= 50,
            "FAST_QUIZ" => false,  // Cần thông tin duration → bỏ qua
            "DAILY_CHALLENGE_7" => false,  // Cần module Daily Challenge → bỏ qua
            _ => false
        };
    }

    public async Task<List<BadgeDto>> GetAllBadgesWithStatusAsync(int userId)
    {
        var allBadges = await _context.Badges.ToListAsync();
        var userBadges = await _context.UserBadges
            .Where(ub => ub.UserId == userId)
            .ToDictionaryAsync(ub => ub.BadgeId, ub => ub.EarnedAt);

        return allBadges.Select(b => new BadgeDto
        {
            BadgeId = b.BadgeId,
            Name = b.Name,
            Description = b.Description,
            IconUrl = b.IconUrl,
            ConditionType = b.ConditionType,
            IsEarned = userBadges.ContainsKey(b.BadgeId),
            EarnedAt = userBadges.TryGetValue(b.BadgeId, out var d) ? d : (DateTime?)null
        }).ToList();
    }

    public async Task<List<BadgeDto>> GetEarnedBadgesAsync(int userId)
    {
        return await _context.UserBadges
            .Where(ub => ub.UserId == userId)
            .OrderByDescending(ub => ub.EarnedAt)
            .Select(ub => new BadgeDto
            {
                BadgeId = ub.Badge.BadgeId,
                Name = ub.Badge.Name,
                Description = ub.Badge.Description,
                IconUrl = ub.Badge.IconUrl,
                ConditionType = ub.Badge.ConditionType,
                IsEarned = true,
                EarnedAt = ub.EarnedAt
            })
            .ToListAsync();
    }

    // Helper class - chỉ dùng nội bộ trong service này
    private class BadgeCheckStats
    {
        public int TotalQuizzes { get; set; }
        public int PerfectScores { get; set; }
        public int CompletedUnits { get; set; }
        public int TotalWords { get; set; }
        public int CurrentStreak { get; set; }
        public int TotalStars { get; set; }
    }
}