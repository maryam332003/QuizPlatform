using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizPlatform.Infrastructure.Services
{
    public class ImageService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ImageService> _logger;

        public ImageService(IHttpContextAccessor httpContextAccessor, ILogger<ImageService> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<string> UploadFileAsync(IFormFile file, string folderName)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    _logger.LogError("File is invalid or empty.");
                    return null;
                }

                string fileExtension = Path.GetExtension(file.FileName).ToLower();
                string fileName = $"{Guid.NewGuid()}{fileExtension}";

                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files", folderName);
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string filePath = Path.Combine(folderPath, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                // إعداد الرابط النهائي
                var request = _httpContextAccessor.HttpContext?.Request;
                if (request == null)
                {
                    _logger.LogError("HTTP Request is null. Cannot build base URL.");
                    return null;
                }

                string baseUrl = $"{request.Scheme}://{request.Host}";
                return $"{baseUrl}/files/{folderName}/{fileName}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file");
                return null;
            }
        }
        public bool DeleteFile(string imageUrl, string folderName)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                _logger.LogWarning("Image URL is null or empty. Nothing to delete.");
                return false;
            }
            try
            {
                var fileName = Path.GetFileName(new Uri(imageUrl).LocalPath);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files", folderName, fileName);

                if (!File.Exists(filePath))
                {
                    _logger.LogWarning($"File not found: {filePath}");
                    return false;
                }

                File.Delete(filePath);
                _logger.LogInformation($"Deleted file: {filePath}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting file: {imageUrl}");
                return false;
            }
        }
    }
}
