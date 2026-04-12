using Application.Common.DTOs;
using MediatR;

namespace Application.Slides.Commands.CreateSlide;

public record CreateSlideCommand(
    string ImageUrl,
    string Title1,
    string Title2,
    string Title3Part1,
    string Title3Part2,
    string? Title3Part3,
    string Title4,
    int Order
) : IRequest<SlideDto>;