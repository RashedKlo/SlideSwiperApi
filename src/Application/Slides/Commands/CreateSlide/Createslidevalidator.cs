using FluentValidation;

namespace Application.Slides.Commands.CreateSlide;

public class CreateSlideValidator : AbstractValidator<CreateSlideCommand>
{
    public CreateSlideValidator()
    {
        RuleFor(x => x.ImageUrl)
            .NotEmpty().WithMessage("Image URL is required.")
            .MaximumLength(500).WithMessage("Image URL must not exceed 500 characters.");

        RuleFor(x => x.Title1)
            .NotEmpty().WithMessage("Title 1 (top text) is required.")
            .MaximumLength(200);

        RuleFor(x => x.Title2)
            .NotEmpty().WithMessage("Title 2 (middle text) is required.")
            .MaximumLength(200);

        RuleFor(x => x.Title3Part1)
            .NotEmpty().WithMessage("Title 3 Part 1 (normal text) is required.")
            .MaximumLength(200);

        RuleFor(x => x.Title3Part2)
            .NotEmpty().WithMessage("Title 3 Part 2 (highlighted red text) is required.")
            .MaximumLength(200);

        RuleFor(x => x.Title3Part3)
            .MaximumLength(200).When(x => x.Title3Part3 is not null);

        RuleFor(x => x.Title4)
            .NotEmpty().WithMessage("Title 4 (bottom text) is required.")
            .MaximumLength(200);

        RuleFor(x => x.Order)
            .GreaterThanOrEqualTo(0).WithMessage("Order must be a non-negative number.");
    }
}