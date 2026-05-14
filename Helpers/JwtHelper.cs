using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using KidsLearn.Configurations;
using KidsLearn.Models;
using Microsoft.IdentityModel.Tokens;

namespace KidsLearn.Helpers;

// Mục đích: Tập trung logic tạo JWT Token
// Tách ra Helper riêng để AuthService không bị quá dài
// Và có thể tái sử dụng nếu sau này cần refresh token
public static class JwtHelper
{
    public static string GenerateToken(User user, JwtSettings jwtSettings)
    {
        // Claims = thông tin được nhúng vào trong token
        // Frontend decode token sẽ đọc được UserId, Username, Role
        // KHÔNG nhúng thông tin nhạy cảm như PasswordHash vào đây
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.Username ?? ""),
            new Claim(ClaimTypes.Role, user.Role ?? ""),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // JWT ID duy nhất
        };

        // Tạo key từ SecretKey trong appsettings
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings.SecretKey));

        var credentials = new SigningCredentials(
            key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtSettings.Issuer,
            audience: jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(jwtSettings.ExpiresInDays),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}