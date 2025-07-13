using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizPlatform.Core.DTOs
{
    public class TextQuestionDto
    {
        public int QuizId { get; set; }
        public string Text { get; set; }
        public string CorrectAnswerText { get; set; }
    }

}
