using QuizPlatform.Core.Common;
using QuizPlatform.Core.DTOs;
using QuizPlatform.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuizPlatform.Core.Interfaces
{
    public interface IQuizService
    {
        Task<ApiResponse> CreateQuiz(QuizDto quizDto);
        Task<ApiResponse> UpdateQuiz(UpdateQuizDto quiz); // هنا التعديل
        Task<ApiResponse> DeleteQuiz(int quizId);
        ApiResponse AllQuizzes(string Name);
        Task<ApiResponse> GetQuizById(int id);
        //ApiResponse QuizQuestionsForUser(int quizId);
        Task<ApiResponse> GetQuizDetailsForUser(int quizId, string userId);
    }
}
