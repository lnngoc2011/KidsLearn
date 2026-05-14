namespace KidsLearn.Configurations
{
    /// <summary>
    /// Map cấu hình JWT từ appsettings.json vào đối tượng này để dễ dàng sử dụng trong ứng dụng.
    /// </summary>
    public class JwtSettings
    {
        public string SecretKey { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int ExpiresInDays { get; set; } = 7;
    }
}
