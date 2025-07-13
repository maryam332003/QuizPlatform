using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizPlatform.Core.DTOs
{
    public class AnswerDto
    {
        public int QuestionId { get; set; }
        public string? AnswerText { get; set; }
        public int? SelectedChoiceId { get; set; }
    }
}
