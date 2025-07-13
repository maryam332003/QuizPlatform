using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizPlatform.Application.Services;
using QuizPlatform.Core.DTOs;
using QuizPlatform.Core.Entities;
using QuizPlatform.Core.Interfaces;
using System.Security.Claims;

namespace QuizPlatform.Web.Controllers
{
    [Authorize]
    public class QuizController : Controller
    {
        private readonly IQuizService _quizService;
        private readonly IAnswerService _answerService;

        public QuizController(IQuizService quizService,IAnswerService answerService)
        {
            _quizService = quizService;
            _answerService = answerService;
        }

        public IActionResult Index(string? Name)
        {
            var quizzes = _quizService.AllQuizzes(Name);
                

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("Partials/_QuizCardsPartial", quizzes.Data);
            }

            return View(quizzes.Data);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int Id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            ViewBag.QuizId = Id;

            var quiz = await _quizService.GetQuizDetailsForUser(Id, userId);
            var data = quiz.Data as QuizDetailsForUserDto;
            if (data.HasAnswered)
            {
                var scoreResult = await _answerService.CheckUserScore(userId, Id);

                if (scoreResult.StatusCode == 200 && scoreResult.Data != null)
                {
                    ViewBag.Score =scoreResult.Message;
                }
            }

            return View(quiz.Data);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]

        public async Task<IActionResult> Create(QuizDto quizDto)
        {
            if (!ModelState.IsValid)
                return View(quizDto);

            var response = await _quizService.CreateQuiz(quizDto);
            return Json(response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]

        public async Task<IActionResult> Edit(UpdateQuizDto quiz)
        {
            if (!ModelState.IsValid)
                return Json(new { statusCode = 400, message = "Invalid data." });

            var response = await _quizService.UpdateQuiz(quiz);
            return Json(new { statusCode = response.StatusCode, message = response.Message });
        }

        // POST: Quiz/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]

        public async Task<IActionResult> Delete(int id)
        {
            var response = await _quizService.DeleteQuiz(id);
            return Json(response);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitQuizAnswers([FromBody] QuizAnswersDto model)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            model.UserId = userId;
            var result = await _answerService.SubmitAnswers(model);
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> UserScore(int quizId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var result = await _answerService.CheckUserScore(userId,quizId);
            return Json(result);
        }

    }
}
