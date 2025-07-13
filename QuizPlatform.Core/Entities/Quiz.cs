namespace QuizPlatform.Core.Entities
{
    public class Quiz
    {
        public int Id{ get; set; }
        public string? Name{ get; set; }
        public string? Image{ get; set; }
        public string? Description{ get; set; }
        public DateTime Date{ get; set; }
        public ICollection<Question>? Questions { get; set; }
        public ICollection<Answer>? Answers { get; set; }
    }
}
