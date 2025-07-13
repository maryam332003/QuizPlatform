using FuzzySharp;
using Microsoft.EntityFrameworkCore;
using QuizPlatform.Application.Validators;
using QuizPlatform.Core.Common;
using QuizPlatform.Core.DTOs;
using QuizPlatform.Core.Entities;
using QuizPlatform.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizPlatform.Application.Services
{
    public class QuestionService: IQuestionService
    {

        private readonly IGenericRepository<Question> _questionRepo;
        private readonly IGenericRepository<Option> _optionRepo;
        public QuestionService(IGenericRepository<Question> questionRepo, IGenericRepository<Option> optionRepo)
        {
            _questionRepo = questionRepo;
            _optionRepo = optionRepo;
        }

        #region Text Questions
        public async Task<ApiResponse> CreateTextQuestion(TextQuestionDto dto)
        {
            var existingQuestions = await _questionRepo.FindAll()
                .Where(q => q.QuizId == dto.QuizId && q.Type == AnswerType.Text)
                .Select(q => q.Text)
                .ToListAsync();

            var similarityChecker = new SimilarityChecker(80); 

            var (isSimilar, similarityScore, similarQuestion) =
                similarityChecker.CheckSimilarity(dto.Text, existingQuestions);

            if (isSimilar)
            {
                return new ApiResponse(400,
                    $"A similar text question already exists: \"{similarQuestion}\".",
                    null);
            }

            var question = new Question
            {
                QuizId = dto.QuizId,
                Text = dto.Text,
                Type = AnswerType.Text,
                CorrectAnswerText = dto.CorrectAnswerText
            };

            _questionRepo.Add(question);
            await _questionRepo.SaveChangesAsync();

            return new ApiResponse(200, "Text question added successfully.", question);
        }

        public async Task<ApiResponse> EditTextQuestion(UpdateTextQuestionDto model)
        {
            var question = await _questionRepo.GetByIdAsync(model.Id);
            if (question == null || question.QuizId != model.QuizId)
                return new ApiResponse(404, "Question not found or does not belong to this quiz.");

            if (question.Type != AnswerType.Text)
                return new ApiResponse(400, "Invalid question type for text edit.");

            var existingQuestions = await _questionRepo.FindAll()
                .Where(q => q.QuizId == model.QuizId && q.Type == AnswerType.Text && q.Id != model.Id)
                .Select(q => q.Text)
                .ToListAsync();

            var similarityChecker = new SimilarityChecker(80);

            var (isSimilar, similarityScore, similarQuestion) =
                similarityChecker.CheckSimilarity(model.Text, existingQuestions);

            if (isSimilar)
            {
                return new ApiResponse(400,
                    $"A similar text question already exists: \"{similarQuestion}\"",
                    null);
            }

            question.Text = model.Text;
            question.CorrectAnswerText = model.CorrectAnswerText;

            await _questionRepo.SaveChangesAsync();
            return new ApiResponse(200, "Text question updated successfully.");
        }


        #endregion

        #region Options Question
        public async Task<ApiResponse> CreateOptionsQuestion(OptionsQuestionDto dto)
        {
            var validator = new OptionsQuestionDtoValidator();
            var validationResult = validator.Validate(dto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => new
                {
                    Field = e.PropertyName,
                    Error = e.ErrorMessage
                });
                var errorMessages = string.Join(" | ", validationResult.Errors
                    .Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));

                return new ApiResponse(400, $"Validation failed: {errorMessages}", errors);
            }

            var existingQuestions = await _questionRepo.FindAll()
                .Where(q => q.QuizId == dto.QuizId && q.Type == AnswerType.Choices)
                .Select(q => q.Text)
                .ToListAsync();
      


            var similarityChecker = new SimilarityChecker(80);

            var (isSimilar, similarityScore, similarQuestion) =
                similarityChecker.CheckSimilarity(dto.Text, existingQuestions);

            if (isSimilar)
            {
                return new ApiResponse(400,
                    $"A similar question already exists: \"{similarQuestion}\" .",
                    null);
            }

            var question = new Question
            {
                QuizId = dto.QuizId,
                Text = dto.Text,
                Type = AnswerType.Choices
            };

            _questionRepo.Add(question);
            await _questionRepo.SaveChangesAsync();

            var options = new List<Option>
            {
                new Option { OptionText = dto.OptionA, IsCorrect = dto.CorrectAnswer == "A", QuestionId = question.Id },
                new Option { OptionText = dto.OptionB, IsCorrect = dto.CorrectAnswer == "B", QuestionId = question.Id },
                new Option { OptionText = dto.OptionC, IsCorrect = dto.CorrectAnswer == "C", QuestionId = question.Id },
                new Option { OptionText = dto.OptionD, IsCorrect = dto.CorrectAnswer == "D", QuestionId = question.Id }
            };

            await _optionRepo.AddRangeAsync(options);
            await _optionRepo.SaveChangesAsync();

            return new ApiResponse(200, "Question added successfully.", null);
        }


        public async Task<ApiResponse> EditChoicesQuestion(UpdateChoicesQuestionDto model)
        {
            var question = await _questionRepo
                .FindAll()
                .Where(q => q.Id == model.Id && q.QuizId == model.QuizId)
                .Select(q => new
                {
                    Question = q,
                    Options = q.Options.ToList()
                })
                .FirstOrDefaultAsync();

            if (question == null)
                return new ApiResponse(404, "Question not found or does not belong to this quiz.");

            if (question.Question.Type != AnswerType.Choices)
                return new ApiResponse(400, "Invalid question type for choices edit.");

            var existingQuestions = await _questionRepo.FindAll()
                .Where(q => q.QuizId == model.QuizId && q.Type == AnswerType.Choices && q.Id != model.Id)
                .Select(q => q.Text)
                .ToListAsync();

            var similarityChecker = new SimilarityChecker(80);

            var (isSimilar, similarityScore, similarQuestion) =
                similarityChecker.CheckSimilarity(model.Text, existingQuestions);

            if (isSimilar)
            {
                return new ApiResponse(400,
                    $"A similar choices question already exists: \"{similarQuestion}\".",
                    null);
            }

            if (model.Options.Any(o => string.IsNullOrWhiteSpace(o.OptionText)))
            {
                return new ApiResponse(400, "All option values must be provided and not empty.");
            }

            // تحقق من التكرار بعد Normalization
            var normalizedOptions = model.Options
                .Select(o => o.OptionText?.Trim().ToLower());

            if (normalizedOptions.Distinct().Count() != normalizedOptions.Count())
            {
                return new ApiResponse(400, "Option values must be unique (case-insensitive).");
            }

            question.Question.Text = model.Text;

            foreach (var inputOption in model.Options)
            {
                var dbOption = question.Options.FirstOrDefault(o => o.Id == inputOption.Id);
                if (dbOption != null)
                {
                    dbOption.OptionText = inputOption.OptionText;
                }
            }

            foreach (var opt in question.Options)
            {
                opt.IsCorrect = (opt.Id == model.CorrectOptionIndex);
            }

            await _questionRepo.SaveChangesAsync();

            return new ApiResponse(200, "Choices question updated successfully.");
        }


        #endregion

        public async Task<ApiResponse> DeleteQuestion(int questionId, int quizId)
        {
            var question = await _questionRepo.GetByIdAsync(questionId);

            if (question == null)
                return new ApiResponse(404, "Question not found.");

            if (question.QuizId != quizId)
                return new ApiResponse(400, "This question does not belong to this quiz.");
            _questionRepo.Delete(question);
            await _questionRepo.SaveChangesAsync();

            return new ApiResponse(200, "Question deleted successfully.");
        }
        public async Task<ApiResponse> QuizQuestions(int quizId)
        {
            var questions =await _questionRepo
                            .FindAll().Where(q => q.QuizId == quizId)
                            .Include(q => q.Options)
                            .Select(q => new QuestionWithOptionsDto
                            {
                                Id = q.Id,
                                Text = q.Text,
                                CorrectAnswerText=q.CorrectAnswerText,
                                Type = q.Type,

                                Options = q.Type == AnswerType.Choices
                                    ? q.Options.Select(o => new OptionDto
                                    {
                                        Id = o.Id,
                                        OptionText = o.OptionText,
                                        IsCorrect = o.IsCorrect
                                    }).ToList()
                                    : null
                            })
                            .ToListAsync();


            if (questions == null || !questions.Any())
                return new ApiResponse(404, "No questions found for this quiz.");

            return new ApiResponse(200, "Questions retrieved successfully.", questions);
        }      

    }

}
