using System.Text.RegularExpressions;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using UploadApi.Application.Interfaces;

namespace UploadApi.Application.Services
{
    public class UploadFileService : IUploadFileService
    {
        private readonly Cloudinary _cloudinary;
        public UploadFileService(IConfiguration configuration)
        {
            var account = new Account(
                configuration["Cloudinary:CloudName"],
                configuration["Cloudinary:ApiKey"],
                configuration["Cloudinary:ApiSecret"]
            );
            _cloudinary = new Cloudinary(account);
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File không hợp lệ");

            await using var stream = file.OpenReadStream();
            string generatedFileName = GenerateFileName(file.FileName);
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(generatedFileName, stream),
                Folder = "eBlog",
                PublicId = generatedFileName
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return uploadResult.SecureUrl.AbsoluteUri;
        }

        private string GenerateFileName(string originalFileName)
        {
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalFileName);
            var fileExtension = Path.GetExtension(originalFileName);

            fileNameWithoutExtension = RemoveInvalidCharacters(fileNameWithoutExtension);
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var uniqueId = Guid.NewGuid().ToString("N").Substring(0, 8);

            return $"eBlog_{fileNameWithoutExtension}_{timestamp}_{uniqueId}{fileExtension}";
        }

        private string RemoveInvalidCharacters(string input)
        {
            return Regex.Replace(input, @"[^a-zA-Z0-9-_]", "_");
        }
    }
}
