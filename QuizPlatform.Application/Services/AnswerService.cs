using FuzzySharp;
using Microsoft.EntityFrameworkCore;
using QuizPlatform.Core.Common;
using QuizPlatform.Core.DTOs;
using QuizPlatform.Core.Entities;
using QuizPlatform.Core.Interfaces;

namespace QuizPlatform.Application.Services
{
    public class AnswerService:IAnswerService
    {
        private readonly IGenericRepository<Answer> _answerRepo;
        private readonly IGenericRepository<Question> _questionRepo;
        private readonly IGenericRepository<Option> _optionRepo;

        public AnswerService(IGenericRepository<Answer> answerRepo, IGenericRepository<Question> questionRepo, IGenericRepository<Option> optionRepo)
        {
            _answerRepo = answerRepo;
            _questionRepo = questionRepo;
            _optionRepo = optionRepo;
        }
        public async Task<ApiResponse> SubmitAnswers(QuizAnswersDto model)
        {
            if (model == null || model.Answers == null || !model.Answers.Any())
                return new ApiResponse(400, "No answers provided.");

            var answers = new List<Answer>();

            foreach (var answerDto in model.Answers)
            {
                var answer = new Answer
                {
                    UserId = model.UserId,
                    QuizId = model.QuizId,
                    QuestionId = answerDto.QuestionId,
                    AnswerText = answerDto.AnswerText,
                    SelectedChoiceId = answerDto.SelectedChoiceId
                };
                answers.Add(answer);
            }

            await _answerRepo.AddRangeAsync(answers);
            await _answerRepo.SaveChangesAsync();

            return new ApiResponse(200, "Answers submitted successfully.");
        }
        public async Task<ApiResponse> CheckUserScoreA(string userId, int quizId)
        {
            var userAnswersQuery = from a in _answerRepo.FindAll()
                                   where a.UserId == userId
                                   join q in _questionRepo.FindAll().Where(q => q.QuizId == quizId)
                                       on a.QuestionId equals q.Id
                                   join o in _optionRepo.FindAll()
                                       on q.Id equals o.QuestionId into optionsJoin
                                   select new
                                   {
                                       Answer = a,
                                       Question = q,
                                       Options = optionsJoin.ToList()
                                   };

            var userAnswers = await userAnswersQuery.ToListAsync();

            if (!userAnswers.Any())
                return new ApiResponse(404, "No answers found for this quiz.");

            int correctCount = 0;

            foreach (var item in userAnswers)
            {
                if (item.Question.Type == AnswerType.Text)
                {
                    if (!string.IsNullOrWhiteSpace(item.Answer.AnswerText) &&
                        !string.IsNullOrWhiteSpace(item.Question.CorrectAnswerText))
                    {
                        int similarityScore = Fuzz.Ratio(
                            item.Answer.AnswerText.Trim().ToLower(),
                            item.Question.CorrectAnswerText.Trim().ToLower());

                        if (similarityScore >= 80) 
                        {
                            correctCount++;
                        }
                    }
                }
                else if (item.Question.Type == AnswerType.Choices)
                {
                    var correctOption = item.Options.FirstOrDefault(o => o.IsCorrect);
                    if (correctOption != null &&
                        item.Answer.SelectedChoiceId.HasValue &&
                        item.Answer.SelectedChoiceId == correctOption.Id)
                    {
                        correctCount++;
                    }
                }
            }

            int totalQuestions = userAnswers.Count;

            return new ApiResponse(200, $"You got {correctCount} out of {totalQuestions} correct!", new
            {
                TotalQuestions = totalQuestions,
                CorrectAnswers = correctCount
            });
        }
    }
}
