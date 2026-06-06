namespace KidsLearn.DTOs.Auth;

// Dữ liệu trả về sau khi đăng nhập/đăng ký thành công
// Chỉ trả về thông tin cần thiết — KHÔNG trả về PasswordHash
public class AuthResponseDto
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }  // Thời điểm token hết hạn
}