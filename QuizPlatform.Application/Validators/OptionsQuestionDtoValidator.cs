using FluentValidation;
using QuizPlatform.Core.DTOs;

namespace QuizPlatform.Application.Validators
{


    public class OptionsQuestionDtoValidator : AbstractValidator<OptionsQuestionDto>
    {
        public OptionsQuestionDtoValidator()
        {
            RuleFor(x => x.Text)
                .NotEmpty().WithMessage("Question text is required.");

            RuleFor(x => x.CorrectAnswer)
                .Must(x => new[] { "A", "B", "C", "D" }.Contains(x))
                .WithMessage("CorrectAnswer must be one of A, B, C, or D.");

            RuleFor(x => x)
                .Must(HaveUniqueOptions)
                .WithMessage("Option values must be unique.");
        }

        private bool HaveUniqueOptions(OptionsQuestionDto dto)
        {
            var options = new[] { dto.OptionA, dto.OptionB, dto.OptionC, dto.OptionD }
                .Where(o => !string.IsNullOrWhiteSpace(o)) 
                .Select(o => o.Trim().ToLower());

            return options.Distinct().Count() == options.Count();
        }

    }

}
