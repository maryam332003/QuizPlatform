using Microsoft.AspNetCore.Http;
using QuizPlatform.Core.Entities;

namespace QuizPlatform.Core.DTOs
{
    public class QuizDto
    {
        public string Name { get; set; } = string.Empty;
        public IFormFile? Image { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
