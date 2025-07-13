using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizPlatform.Core.Interfaces;

public class AdminController : Controller
{
    private readonly IQuizService _quizService;

    public AdminController(IQuizService quizService)
    {
        _quizService = quizService;
    }
    [Authorize(Roles ="admin")]

    public IActionResult Index()
    {
        var response = _quizService.AllQuizzes(null);
        return View(response.Data);
    }
}
