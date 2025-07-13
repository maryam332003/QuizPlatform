using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizPlatform.Core.Entities
{
    public class Answer
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }

        public int QuestionId { get; set; }
        public Question Question { get; set; }
        
        public int QuizId { get; set; }
        public Quiz Quiz { get; set; }

        public string? AnswerText { get; set; } 
        public int? SelectedChoiceId { get; set; } 
    }

}
