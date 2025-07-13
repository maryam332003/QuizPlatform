using QuizPlatform.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizPlatform.Core.DTOs
{
    public class QuestionAnswerDto
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public AnswerType Type { get; set; }
        public List<OptionDto> Options { get; set; }
        public string? UserAnswerText { get; set; }
        public int? SelectedOptionId { get; set; }
    }
}
