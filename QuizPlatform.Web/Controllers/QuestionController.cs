using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizPlatform.Application.Services;
using QuizPlatform.Core.DTOs;
using QuizPlatform.Core.Entities;
using QuizPlatform.Core.Interfaces;

namespace QuizPlatform.Web.Controllers
{
    public class QuestionController : Controller
    {
        private readonly IQuestionService _questionService;

        public QuestionController(IQuestionService questionService)
        {
            _questionService = questionService;
        }
        [Authorize]
        public async Task<IActionResult> Index(int quizId)
        {
            var response =await _questionService.QuizQuestions(quizId);
            ViewBag.QuizId = quizId;

            if (response.StatusCode == 200)
            {
                ViewBag.Questions = response.Data;
            }
            else
            {
                ViewBag.Questions = new List<Question>(); 
            }

            return View();
        }

        #region Text Questions
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateText(TextQuestionDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.QuizId = dto.QuizId;
                return View("Add");
            }

            var response = await _questionService.CreateTextQuestion(dto);
            return Json(response);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> UpdateTextQuestion([FromBody] UpdateTextQuestionDto request)
        {
            var response = await _questionService.EditTextQuestion(request);
            return Json(response);
        }
        #endregion

        #region Choices Questions
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateOptions(OptionsQuestionDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.QuizId = dto.QuizId;
                return View("Add");
            }

            var response = await _questionService.CreateOptionsQuestion(dto);
            return Json(response);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> UpdateChoicesQuestion([FromBody] UpdateChoicesQuestionDto model)
        {
            if (!ModelState.IsValid)
                return Json(new { StatusCode = 400, Message = "Invalid data" });

            var response = await _questionService.EditChoicesQuestion(model);
            return Json(response);
        }
        #endregion

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int questionId, int quizId)
        {
            var response=await _questionService.DeleteQuestion(questionId,quizId);

            return Json(response);
        }
    }

}
