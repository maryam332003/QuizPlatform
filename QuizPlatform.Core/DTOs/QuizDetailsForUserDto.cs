using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizPlatform.Core.DTOs
{
    public class QuizDetailsForUserDto
    {
        public int QuizId { get; set; }
        public List<QuestionAnswerDto> Questions { get; set; }
        public bool HasAnswered { get; set; }
    }
}
