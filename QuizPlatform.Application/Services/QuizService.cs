using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QuizPlatform.Core.Common;
using QuizPlatform.Core.DTOs;
using QuizPlatform.Core.Entities;
using QuizPlatform.Core.Interfaces;
using QuizPlatform.Infrastructure.Services;

public class QuizService : IQuizService
{
    private readonly IGenericRepository<Quiz> _quizRepository;
    private readonly IGenericRepository<Question> _QuestionRepo;
    private readonly ILogger<QuizService> _logger;
    private readonly ImageService _imageService;
    private readonly IGenericRepository<Answer> _answerRepo;
    private readonly IGenericRepository<Option> _optionRepo;

    public QuizService(IGenericRepository<Quiz> quizRepository,
                       ILogger<QuizService> logger,
                       ImageService imageService,
                       IGenericRepository<Answer> AnswerRepo,
                       IGenericRepository<Option> OptionRepo,
                       IGenericRepository<Question> QuestionRepo)
    {
        _quizRepository = quizRepository;
        _logger = logger;
        _imageService = imageService;
        _answerRepo = AnswerRepo;
        _optionRepo = OptionRepo;
        _QuestionRepo = QuestionRepo;
    }

    #region Quiz CRUD
    public async Task<ApiResponse> CreateQuiz(QuizDto quizDto)
    {
        var quiz = quizDto.Adapt<Quiz>();

        string imagePath = null;

        if (quizDto.Image != null)
        {
            imagePath = await _imageService.UploadFileAsync(quizDto.Image, "QuizzesImage");
        }

        quiz.Image = imagePath;
        _quizRepository.Add(quiz);
        await _quizRepository.SaveChangesAsync();

        return new ApiResponse(200, "Quiz created successfully.", quiz);
    }
    public async Task<ApiResponse> UpdateQuiz(UpdateQuizDto quizDto)
    {
        var existingQuiz = await _quizRepository.GetByIdAsync(quizDto.Id);
        if (existingQuiz == null)
            return new ApiResponse(404, "Quiz not found.");

        existingQuiz.Name = quizDto.Name;
        existingQuiz.Description = quizDto.Description;

        if (quizDto.Image != null && quizDto.Image.Length > 0)
        {
            if (!string.IsNullOrEmpty(existingQuiz.Image))
            {
                _imageService.DeleteFile(existingQuiz.Image, "QuizzesImage");
            }

            var imagePath = await _imageService.UploadFileAsync(quizDto.Image, "QuizzesImage");
            existingQuiz.Image = imagePath;
        }

        await _quizRepository.SaveChangesAsync();

        return new ApiResponse(200, "Quiz updated successfully.", existingQuiz);
    }
    public async Task<ApiResponse> DeleteQuiz(int quizId)
    {
        var quiz = await _quizRepository.GetByIdAsync(quizId);
        if (quiz == null)
            return new ApiResponse(404, "Quiz not found.");

        var questions = _QuestionRepo.FindBy(q => q.QuizId == quizId).ToList();
        if (questions != null && questions.Any())
        {
            _QuestionRepo.DeleteRange(questions);
        }

        if (!string.IsNullOrEmpty(quiz.Image))
        {
            _imageService.DeleteFile(quiz.Image, "QuizzesImage");
        }

        _quizRepository.Delete(quiz);
        await _quizRepository.SaveChangesAsync();

        return new ApiResponse(200, "Quiz deleted successfully.");
    }
    #endregion


    public ApiResponse AllQuizzes(string Name)
    {
        var quizzes = _quizRepository.GetAll();
        if (!string.IsNullOrEmpty(Name))
        {
            quizzes = quizzes.Where(q => q.Name.ToLower().Contains(Name.ToLower()));
        }
        return new ApiResponse(200, "Quizzes retrieved successfully.", quizzes.ToList());
    }
    public async Task<ApiResponse> GetQuizById(int id)
    {
        var quiz = await _quizRepository.GetByIdAsync(id);
        if (quiz == null)
            return new ApiResponse(404, "Quiz not found.");

        return new ApiResponse(200, "Quiz retrieved successfully.", quiz);
    }
    public async Task<ApiResponse> GetQuizDetailsForUser(int quizId, string userId)
    {
        var questions =
            from q in _QuestionRepo.FindAll() where q.QuizId == quizId
            join a in _answerRepo.FindAll()
                .Where(ans => ans.UserId == userId && ans.QuizId == quizId)
                on q.Id equals a.QuestionId into answerJoin
            from userAnswer in answerJoin.DefaultIfEmpty()
            join o in _optionRepo.FindAll()
                on q.Id equals o.QuestionId into optionsJoin

            select new QuestionAnswerDto
            {
                Id = q.Id,
                Text = q.Text,
                Type = q.Type,

                Options = optionsJoin.Select(opt => new OptionDto
                {
                    Id = opt.Id,
                    OptionText = opt.OptionText
                }).ToList(),

                UserAnswerText = userAnswer.AnswerText,
                SelectedOptionId = userAnswer.SelectedChoiceId
            };

        var hasAnswered =await _answerRepo.AnyAsync(ans => ans.UserId == userId && ans.QuizId == quizId);

        var result = new QuizDetailsForUserDto
        {
            QuizId = quizId,
            Questions = questions.ToList(),
            HasAnswered = hasAnswered
        };

        return new ApiResponse(200, "Questions retrieved successfully.", result);
    }

}
