using KidsLearn.Data;
using KidsLearn.DTOs.Progress;
using KidsLearn.Models;
using KidsLearn.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KidsLearn.Services;

public class ProgressService : IProgressService
{
    private readonly KidsLearnDbContext _context;

    public ProgressService(KidsLearnDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Lưu tiến độ học tập
    /// ✨ FIX QUAN TRỌNG: TRƯỚC ĐÂY UPDATE record cũ (sai logic)
    /// NAY: INSERT 1 record mới mỗi lần làm bài → giữ được lịch sử
    /// </summary>
    public async Task<ProgressResponseDto> SaveProgressAsync(int userId, SaveProgressDto dto)
    {
        // Kiểm tra Unit tồn tại
        var unit = await _context.Units.FindAsync(dto.UnitId);
        if (unit == null)
            throw new KeyNotFoundException($"Không tìm thấy Unit với ID = {dto.UnitId}");

        // ✨ FIX: Luôn tạo record mới (không update)
        var progress = new LearningProgress
        {
            UserId = userId,
            UnitId = dto.UnitId,
            Score = dto.Score,
            CompletedAt = DateTime.UtcNow
        };

        _context.LearningProgresses.Add(progress);
        await _context.SaveChangesAsync();

        return new ProgressResponseDto
        {
            ProgressId = progress.ProgressId,
            UnitId = progress.UnitId,
            UnitTitle = unit.Title,
            Score = progress.Score,
            CompletedAt = progress.CompletedAt ?? DateTime.UtcNow
        };
    }

    /// <summary>
    /// Lấy toàn bộ lịch sử làm bài của user
    /// </summary>
    public async Task<List<ProgressResponseDto>> GetByUserAsync(int userId)
    {
        return await _context.LearningProgresses
            .Where(p => p.UserId == userId)
            .Include(p => p.Unit)
            .OrderByDescending(p => p.CompletedAt ?? DateTime.MinValue)
            .Select(p => new ProgressResponseDto
            {
                ProgressId = p.ProgressId,
                UnitId = p.UnitId,
                UnitTitle = p.Unit.Title,
                Score = p.Score,
                CompletedAt = p.CompletedAt ?? DateTime.MinValue
            })
            .ToListAsync();
    }

    /// <summary>
    /// ✨ MỚI: Lấy điểm cao nhất của user trên 1 Unit
    /// Dùng để hiển thị "Best Score" trên card Unit
    /// </summary>
    public async Task<ProgressResponseDto?> GetBestScoreAsync(int userId, int unitId)
    {
        var best = await _context.LearningProgresses
            .Where(p => p.UserId == userId && p.UnitId == unitId)
            .Include(p => p.Unit)
            .OrderByDescending(p => p.Score)
            .ThenByDescending(p => p.CompletedAt)
            .FirstOrDefaultAsync();

        if (best == null) return null;

        return new ProgressResponseDto
        {
            ProgressId = best.ProgressId,
            UnitId = best.UnitId,
            UnitTitle = best.Unit.Title,
            Score = best.Score,
            CompletedAt = best.CompletedAt ?? DateTime.MinValue
        };
    }
}