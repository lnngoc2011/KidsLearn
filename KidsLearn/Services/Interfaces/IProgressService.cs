using KidsLearn.DTOs.Progress;

namespace KidsLearn.Services.Interfaces;

public interface IProgressService
{
    // Lưu tiến độ (mỗi lần là 1 record mới để theo dõi lịch sử)
    Task<ProgressResponseDto> SaveProgressAsync(int userId, SaveProgressDto dto);

    // Lấy lịch sử của user
    Task<List<ProgressResponseDto>> GetByUserAsync(int userId);

    // Lấy điểm cao nhất của user trên 1 Unit cụ thể
    Task<ProgressResponseDto?> GetBestScoreAsync(int userId, int unitId);
}