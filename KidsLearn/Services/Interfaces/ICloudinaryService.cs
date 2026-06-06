namespace KidsLearn.Services.Interfaces
{
    public interface ICloudinaryService
    {
        Task<string> UploadImageAsync(IFormFile file);
        Task DeleteImageAsync(string imageUrl);
        String GetPublicIdFromUrl(string imageUrl);
    }
}
