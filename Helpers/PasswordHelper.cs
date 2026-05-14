namespace KidsLearn.Helpers;

// Mục đích: Mã hóa mật khẩu bằng BCrypt
// BCrypt tự động thêm "salt" ngẫu nhiên vào mỗi lần hash
// → Cùng 1 mật khẩu sẽ cho ra hash khác nhau mỗi lần
// → Chống được Rainbow Table Attack
public static class PasswordHelper
{
    // workFactor = 12: độ phức tạp tính toán (2^12 vòng lặp)
    // Cao hơn = an toàn hơn nhưng chậm hơn. 12 là chuẩn khuyến nghị
    private const int WorkFactor = 12;

    public static string HashPassword(string password)
        => BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);

    // BCrypt.Verify tự extract salt từ hash để so sánh
    // Không cần lưu salt riêng như SHA256
    public static bool VerifyPassword(string password, string hashedPassword)
        => BCrypt.Net.BCrypt.Verify(password, hashedPassword);
}