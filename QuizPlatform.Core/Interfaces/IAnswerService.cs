using QuizPlatform.Core.Common;
using QuizPlatform.Core.DTOs;
namespace QuizPlatform.Core.Interfaces
{
    public interface IAnswerService
    {
        Task<ApiResponse> SubmitAnswers(QuizAnswersDto model);
        Task<ApiResponse> CheckUserScore(string userId, int quizId);
    }
}
