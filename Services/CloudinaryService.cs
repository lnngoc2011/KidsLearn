namespace KidsLearn.Services

{
    using CloudinaryDotNet;
    using CloudinaryDotNet.Actions;
    using KidsLearn.Services.Interfaces;

    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }

        // Upload Image
        public async Task<string> UploadImageAsync(IFormFile file)
        {
            using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "your_folder_name"   // optional
            };

            var result = await _cloudinary.UploadAsync(uploadParams);
            return result.SecureUrl.ToString(); // returns the image URL
        }

        // Delete Image
        public async Task DeleteImageAsync(string imageUrl)
        {
            var publicId = GetPublicIdFromUrl(imageUrl);
            var deleteParams = new DeletionParams(publicId);
            await _cloudinary.DestroyAsync(deleteParams);
        }
        public string GetPublicIdFromUrl(string imageUrl)
        {
            // Lấy phần sau "upload/"
            var uri = new Uri(imageUrl);
            var segments = uri.AbsolutePath.Split('/');

            // Tìm index của "upload"
            var uploadIndex = Array.IndexOf(segments, "upload");

            // Bỏ version (v1234567890) nếu có
            var startIndex = uploadIndex + 1;
            if (segments[startIndex].StartsWith("v") &&
                long.TryParse(segments[startIndex].Substring(1), out _))
            {
                startIndex++; // bỏ qua version
            }

            // Ghép lại thành PublicId (bỏ phần extension .jpg/.png)
            var publicIdWithExt = string.Join("/", segments.Skip(startIndex));
            var publicId = Path.GetFileNameWithoutExtension(publicIdWithExt);

            return publicId; // "avatars/abc123"
        }

    }
}
