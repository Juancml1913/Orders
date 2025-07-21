
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace Orders.Backend.Helpers
{
    public class FileStorage : IFileStorage
    {
        private readonly IConfiguration _configuration;

        public FileStorage(IConfiguration configuration)
        {
            _configuration=configuration;
        }
        public async Task RemoveFileAsync(string path, string containerName)
        {
            var account = new Account(
                _configuration["Cloudinary:CloudName"]!.ToString(),
                _configuration["Cloudinary:ApiKey"]!.ToString(),
                _configuration["Cloudinary:ApiSecret"]!.ToString()
            );

            var cloudinary = new Cloudinary(account);
            var deletionParams = new DeletionParams(path);

            await cloudinary.DestroyAsync(deletionParams);

        }

        public async Task<string> SaveFileAsync(byte[] content, string extention, string containerName)
        {
            var account = new Account(
                _configuration["Cloudinary:CloudName"]!.ToString(),
                _configuration["Cloudinary:ApiKey"]!.ToString(),
                _configuration["Cloudinary:ApiSecret"]!.ToString()
            );

            var cloudinary = new Cloudinary(account);

            using var stream = new MemoryStream(content);

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription($"{Guid.NewGuid()}{extention}", stream),
                Folder = containerName // opcional
            };

            var result = await cloudinary.UploadAsync(uploadParams);

            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return result.SecureUrl.ToString();
            }
            throw new Exception($"Error al subir imagen: {result.Error?.Message}");
        }
    }
}
