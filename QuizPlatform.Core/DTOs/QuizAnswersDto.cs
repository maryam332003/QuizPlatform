using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizPlatform.Core.DTOs
{
    public class QuizAnswersDto
    {
        public int QuizId { get; set; }     
        public string? UserId { get; set; }  
        public List<AnswerDto> Answers { get; set; }
    }
}
