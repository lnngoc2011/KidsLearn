using KidsLearn.Data;
using KidsLearn.DTOs.User;
using KidsLearn.Helpers;
using KidsLearn.Models;
using KidsLearn.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KidsLearn.Services;

public class UserService : IUserService
{
    private readonly KidsLearnDbContext _context;
    private readonly ICloudinaryService _cloudinaryService;

    public UserService(KidsLearnDbContext context, ICloudinaryService cloudinaryService) 
    {
        _context = context;
        _cloudinaryService = cloudinaryService;
    }

    /// <summary>
    /// ✨ MỚI: Profile tổng hợp cho Dashboard học sinh
    /// Gộp dữ liệu từ: Users, UserStreak, LearningProgress, UserBadge
    /// </summary>
    public async Task<UserProfileDto?> GetMyProfileAsync(int userId)
    {
        var user = await _context.Users
            .Include(u => u.UserStreak)
            .FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null) return null;

        // Tính tổng Unit đã thử & đã hoàn thành (DISTINCT UnitId)
        var attempted = await _context.LearningProgresses
            .Where(p => p.UserId == userId)
            .Select(p => p.UnitId)
            .Distinct()
            .CountAsync();

        var completed = await _context.LearningProgresses
            .Where(p => p.UserId == userId && p.Score >= 70)
            .Select(p => p.UnitId)
            .Distinct()
            .CountAsync();

        // Điểm trung bình = AVG(MAX(Score) theo UnitId)
        var maxScores = await _context.LearningProgresses
            .Where(p => p.UserId == userId)
            .GroupBy(p => p.UnitId)
            .Select(g => g.Max(x => x.Score))
            .ToListAsync();

        decimal avgScore = maxScores.Count > 0
            ? Math.Round(maxScores.Average(), 2)
            : 0;

        // Tổng số lượt làm quiz
        var totalQuizzes = await _context.LearningProgresses
            .CountAsync(p => p.UserId == userId);

        // Số huy hiệu
        var badgesCount = await _context.UserBadges
            .CountAsync(ub => ub.UserId == userId);

        return new UserProfileDto
        {
            UserId = user.UserId,
            Username = user.Username,
            FullName = user.FullName,
            AvatarUrl = user.AvatarUrl,
            TotalXP = user.TotalXP,
            Level = user.Level,
            LevelName = GetLevelName(user.Level),
            CurrentStreak = user.UserStreak?.CurrentStreak ?? 0,
            LongestStreak = user.UserStreak?.LongestStreak ?? 0,
            TotalUnitsAttempted = attempted,
            TotalUnitsCompleted = completed,
            AverageScore = avgScore,
            TotalQuizzesTaken = totalQuizzes,
            BadgesEarned = badgesCount
        };
    }

    /// <summary>
    /// User đổi mật khẩu - phải nhập mật khẩu cũ để xác thực
    /// </summary>
    public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto dto)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;

        if (!PasswordHelper.VerifyPassword(dto.OldPassword, user.PasswordHash))
            throw new UnauthorizedAccessException("Mật khẩu cũ không đúng!");

        user.PasswordHash = PasswordHelper.HashPassword(dto.NewPassword);
        await _context.SaveChangesAsync();
        return true;
    }

    // ===== ADMIN =====

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        return await _context.Users
            .OrderByDescending(u => u.CreatedAt)
            .Select(u => new UserDto
            {
                UserId = u.UserId,
                Username = u.Username,
                FullName = u.FullName,
                AvatarUrl = u.AvatarUrl,
                Role = u.Role,
                CreatedAt = u.CreatedAt,
                TotalXP = u.TotalXP,
                Level = u.Level
            })
            .ToListAsync();
    }

    public async Task<UserDto?> GetUserByIdAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        return user == null ? null : MapToDto(user);
    }

    public async Task<UserDto?> ChangeRoleAsync(int userId, ChangeRoleDto dto)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return null;

        user.Role = dto.Role;
        await _context.SaveChangesAsync();
        return MapToDto(user);
    }

    public async Task<bool> DeleteUserAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;
        if(user.AvatarUrl != null && user.AvatarUrl.Contains("res.cloudinary.com"))
        {
            var publicId = _cloudinaryService.GetPublicIdFromUrl(user.AvatarUrl);
            await _cloudinaryService.DeleteImageAsync(publicId);
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ResetPasswordAsync(int userId, string newPassword)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;

        user.PasswordHash = PasswordHelper.HashPassword(newPassword);
        await _context.SaveChangesAsync();
        return true;
    }

    // ===== Helpers =====

    private static UserDto MapToDto(Models.User u) => new()
    {
        UserId = u.UserId,
        Username = u.Username,
        FullName = u.FullName,
        AvatarUrl = u.AvatarUrl,
        Role = u.Role,
        CreatedAt = u.CreatedAt,
        TotalXP = u.TotalXP,
        Level = u.Level
    };

    // Quy ước Level → Tên cấp độ
    private static string GetLevelName(int level) => level switch
    {
        1 => "Beginner",
        2 => "Explorer",
        3 => "Smart Kid",
        _ => "Vocab Master"
    };

    public async Task<UserDto?> UpdateProfileAsync(int userId, string fullName)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return null;

        if (!string.IsNullOrEmpty(fullName))
            user.FullName = fullName;
        await _context.SaveChangesAsync();
        return MapToDto(user);
    }

    public async Task<UserDto?> UpdateAvatarAsync(int userId, IFormFile file)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return null;

        // Xóa ảnh cũ trên Cloudinary
        if (!string.IsNullOrEmpty(user.AvatarUrl))
        {
            var oldPublicId = _cloudinaryService.GetPublicIdFromUrl(user.AvatarUrl);
            await _cloudinaryService.DeleteImageAsync(oldPublicId);
        }

        // Upload ảnh mới
        var newAvatarUrl = await _cloudinaryService.UploadImageAsync(file);

        user.AvatarUrl = newAvatarUrl;
        await _context.SaveChangesAsync();

        return MapToDto(user);
    }
}