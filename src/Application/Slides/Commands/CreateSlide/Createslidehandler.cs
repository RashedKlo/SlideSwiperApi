using Application.Common.DTOs;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Slides.Commands.CreateSlide;

public class CreateSlideHandler : IRequestHandler<CreateSlideCommand, SlideDto>
{
    private readonly ISlideRepository _repository;
    private readonly ICacheService _cache;

    public CreateSlideHandler(ISlideRepository repository, ICacheService cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<SlideDto> Handle(CreateSlideCommand request, CancellationToken cancellationToken)
    {
        var slide = Slide.Create(
            request.ImageUrl,
            request.Title1,
            request.Title2,
            request.Title3Part1,
            request.Title3Part2,
            request.Title3Part3,
            request.Title4,
            request.Order
        );

        await _repository.AddAsync(slide, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        // Invalidate list cache since data changed
        await _cache.RemoveAsync("slides:all", cancellationToken);

        return new SlideDto(
            slide.Id, slide.ImageUrl, slide.Title1, slide.Title2,
            slide.Title3Part1, slide.Title3Part2, slide.Title3Part3,
            slide.Title4, slide.Order, slide.IsActive, slide.CreatedAt, slide.UpdatedAt
        );
    }
}