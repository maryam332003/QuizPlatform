using QuizPlatform.Core.Entities;

namespace QuizPlatform.Core.DTOs
{
    public class QuestionWithOptionsDto
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string? CorrectAnswerText { get; set; }
        public AnswerType Type { get; set; }
        public int QuizId { get; set; }
        public List<OptionDto>? Options { get; set; }
    }
}
