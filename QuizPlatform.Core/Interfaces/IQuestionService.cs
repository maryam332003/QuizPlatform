using QuizPlatform.Core.Common;
using QuizPlatform.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizPlatform.Core.Interfaces
{
    public interface IQuestionService
    {
        Task<ApiResponse> CreateTextQuestion(TextQuestionDto dto);
        Task<ApiResponse> EditTextQuestion(UpdateTextQuestionDto model);
        Task<ApiResponse> CreateOptionsQuestion(OptionsQuestionDto dto);
        Task<ApiResponse> EditChoicesQuestion(UpdateChoicesQuestionDto model);
        Task<ApiResponse> DeleteQuestion(int questionId, int quizId);
        Task<ApiResponse> QuizQuestions(int quizId);
    }
}
