using QuizPlatform.Core.Common;
using QuizPlatform.Core.DTOs;

namespace QuizPlatform.Core.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse> Register(RegisterDto model);
        Task<ApiResponse> Login(LoginDto model);
        Task LogOut();
    }
}
