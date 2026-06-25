using FluentValidation;
using PRN232.LMS.Services.Models.Semesters;
namespace PRN232.LMS.Services.Validators
{
    public class SemesterRequestValidator : AbstractValidator<SemesterRequest>
    {
        public SemesterRequestValidator()
        {
            RuleFor(x => x.SemesterName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.EndDate)
                .GreaterThan(x => x.StartDate)
                .WithMessage("EndDate must be later than StartDate.");
        }
    }
}
