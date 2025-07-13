using Microsoft.EntityFrameworkCore;
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

            question.Text = model.Text;
            question.CorrectAnswerText = model.CorrectAnswerText;

            await _questionRepo.SaveChangesAsync();
            return new ApiResponse(200, "Text question updated successfully.");
        }

        #endregion

        #region Options Question
        public async Task<ApiResponse> CreateOptionsQuestion(OptionsQuestionDto dto)
        {
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

            return new ApiResponse(200, "Options question added successfully.", null);
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
