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
            if (string.IsNullOrEmpty(publicId)) return;

            var deleteParams = new DeletionParams(publicId);
            await _cloudinary.DestroyAsync(deleteParams);
        }
        public string GetPublicIdFromUrl(string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return string.Empty;

            if (!Uri.TryCreate(imageUrl, UriKind.Absolute, out var uri))
                return string.Empty;
            var segments = uri.AbsolutePath.Split('/');

            // Tìm index của "upload"
            var uploadIndex = Array.IndexOf(segments, "upload");
            if (uploadIndex == -1) return string.Empty;

            var startIndex = uploadIndex + 1;

            // Bỏ qua version nếu có (v1780219511)
            if (startIndex < segments.Length
                && segments[startIndex].StartsWith("v")
                && long.TryParse(segments[startIndex].Substring(1), out _))
            {
                startIndex++;
            }

            // Ghép lại toàn bộ path (giữ folder)
            // VD: ["your_folder_name", "rz78co7acxgk41g4t8bz.png"]
            var parts = segments.Skip(startIndex).ToArray();

            // Bỏ extension ở phần tử cuối
            parts[parts.Length - 1] = Path.GetFileNameWithoutExtension(parts[parts.Length - 1]);

            // Kết quả: "your_folder_name/rz78co7acxgk41g4t8bz"
            return string.Join("/", parts);
        }

    }
}
