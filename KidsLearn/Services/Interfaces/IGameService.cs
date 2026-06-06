using KidsLearn.DTOs.Game;

namespace KidsLearn.Services.Interfaces;

// Service xử lý logic GAME (Streak, XP, Badges) cho user
// Được gọi tự động sau khi học sinh nộp Quiz
public interface IGameService
{
    /// <summary>
    /// Cập nhật Streak khi user học hôm nay.
    /// Logic: cùng ngày → no-op; hôm qua → +1; khác → reset về 1
    /// </summary>
    Task<StreakUpdateResultDto> UpdateStreakAsync(int userId);

    /// <summary>
    /// Cộng XP cho user, tự động tính lại Level
    /// </summary>
    Task<XPUpdateResultDto> AddXPAsync(int userId, int xp);

    /// <summary>
    /// Kiểm tra tất cả huy hiệu user có thể đạt sau lần làm quiz này
    /// Trả về DANH SÁCH huy hiệu MỚI đạt được (chưa có trước đó)
    /// </summary>
    Task<List<BadgeDto>> CheckAndAwardBadgesAsync(int userId);

    /// <summary>
    /// Lấy toàn bộ huy hiệu (kèm trạng thái đạt được của user)
    /// </summary>
    Task<List<BadgeDto>> GetAllBadgesWithStatusAsync(int userId);

    /// <summary>
    /// Lấy chỉ những huy hiệu user đã đạt
    /// </summary>
    Task<List<BadgeDto>> GetEarnedBadgesAsync(int userId);
}