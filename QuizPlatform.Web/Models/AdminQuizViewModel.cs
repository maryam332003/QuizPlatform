namespace QuizPlatform.Web.Models
{
    public class AdminQuizViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public DateTime QuizDate { get; set; }
        public int QuestionsCount { get; set; }
    }
}
