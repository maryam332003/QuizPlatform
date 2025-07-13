namespace QuizPlatform.Core.Entities
{
    public class Question
    {
        public int Id { get; set; }
        //if the question is Text
        public string? Text { get; set; }
        public string? CorrectAnswerText { get; set; }
        public AnswerType Type { get; set; }  
        public int QuizId { get; set; }
        public Quiz Quiz { get; set; }
        //if the question is Options
        public ICollection<Option>? Options { get; set; } 
        public ICollection<Answer>? Answers{ get; set; } 
    }

    public enum AnswerType
    {
        Text,
        Choices
    }
}
