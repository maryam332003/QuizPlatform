using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizPlatform.Core.DTOs
{
    public class UpdateChoicesQuestionDto
    {
        public int Id { get; set; }
        public int QuizId { get; set; }
        public string Text { get; set; }
        public List<OptionDto> Options { get; set; }
        public int CorrectOptionIndex { get; set; }
    }
}
