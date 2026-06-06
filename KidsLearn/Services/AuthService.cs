using KidsLearn.Configurations;
using KidsLearn.Data;
using KidsLearn.DTOs.Auth;
using KidsLearn.Helpers;
using KidsLearn.Models;
using KidsLearn.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace KidsLearn.Services;

public class AuthService : IAuthService
{
    private readonly KidsLearnDbContext _context;
    private readonly JwtSettings _jwtSettings;

    public AuthService(
        KidsLearnDbContext context,
        IOptions<JwtSettings> jwtOptions)
    {
        _context = context;
        _jwtSettings = jwtOptions.Value;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        // Kiểm tra username đã tồn tại chưa (case-insensitive)
        var exists = await _context.Users
            .AnyAsync(u => u.Username.ToLower() == request.Username.ToLower());

        if (exists)
            throw new InvalidOperationException("Tên đăng nhập đã tồn tại!");

        var user = new User
        {
            Username = request.Username.Trim(),
            // Hash mật khẩu trước khi lưu vào DB
            // TUYỆT ĐỐI không lưu plain text
            PasswordHash = PasswordHelper.HashPassword(request.Password),
            Role = "Student",   // Mặc định khi đăng ký là Student
            CreatedAt = DateTime.UtcNow,
            TotalXP = 0,
            Level = 1
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync(); // Lưu User trước để có UserId

        // ✅ Tạo UserStreak cho user mới
        var streak = new UserStreak
        {
            UserId = user.UserId,
            CurrentStreak = 0,
            LongestStreak = 0,
            LastStudyDate = null
        };
        _context.UserStreaks.Add(streak);

        await _context.SaveChangesAsync(); // Lưu các bảng liên quan

        return BuildAuthResponse(user);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        // Tìm user theo username
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username.ToLower()
                == request.Username.ToLower());

        // Luôn kiểm tra password dù user null
        // Tránh "timing attack" — phân biệt user không tồn tại vs sai mật khẩu
        if (user == null || !PasswordHelper.VerifyPassword(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Tên đăng nhập hoặc mật khẩu không đúng!");

        return BuildAuthResponse(user);
    }

    // Tách ra method riêng vì cả Register lẫn Login đều cần build response
    private AuthResponseDto BuildAuthResponse(User user)
    {
        var token = JwtHelper.GenerateToken(user, _jwtSettings);

        return new AuthResponseDto
        {
            UserId = user.UserId,
            Username = user.Username ?? "",
            Role = user.Role ?? "",
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.ExpiresInDays)
        };
    }
}