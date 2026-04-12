using FluentValidation;

namespace Application.Slides.Commands.UpdateSlide;

public class UpdateSlideValidator : AbstractValidator<UpdateSlideCommand>
{
    public UpdateSlideValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Slide ID is required.");

        RuleFor(x => x.ImageUrl)
            .NotEmpty().WithMessage("Image URL is required.")
            .MaximumLength(500);

        RuleFor(x => x.Title1)
            .NotEmpty().WithMessage("Title 1 is required.")
            .MaximumLength(200);

        RuleFor(x => x.Title2)
            .NotEmpty().WithMessage("Title 2 is required.")
            .MaximumLength(200);

        RuleFor(x => x.Title3Part1)
            .NotEmpty().WithMessage("Title 3 Part 1 is required.")
            .MaximumLength(200);

        RuleFor(x => x.Title3Part2)
            .NotEmpty().WithMessage("Title 3 Part 2 is required.")
            .MaximumLength(200);

        RuleFor(x => x.Title3Part3)
            .MaximumLength(200).When(x => x.Title3Part3 is not null);

        RuleFor(x => x.Title4)
            .NotEmpty().WithMessage("Title 4 is required.")
            .MaximumLength(200);

        RuleFor(x => x.Order)
            .GreaterThanOrEqualTo(0);
    }
}