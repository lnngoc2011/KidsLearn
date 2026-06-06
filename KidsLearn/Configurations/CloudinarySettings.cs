namespace KidsLearn.Configurations
{
    /// <summary>
    /// Dùng để lưu trữ cấu hình Cloudinary, được bind từ appsettings.json
    /// </summary>
    public class CloudinarySettings
    {
        public string CloudName { get; set; }
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
    }
}
