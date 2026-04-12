using MediatR;

namespace Application.Slides.Commands.DeleteSlide;

public record DeleteSlideCommand(Guid Id) : IRequest<bool>;