using Application.Common.DTOs;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Slides.Commands.UpdateSlide;

public class UpdateSlideHandler : IRequestHandler<UpdateSlideCommand, SlideDto?>
{
    private readonly ISlideRepository _repository;
    private readonly ICacheService _cache;

    public UpdateSlideHandler(ISlideRepository repository, ICacheService cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<SlideDto?> Handle(UpdateSlideCommand request, CancellationToken cancellationToken)
    {
        var slide = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (slide is null) return null;

        slide.Update(
            request.ImageUrl,
            request.Title1,
            request.Title2,
            request.Title3Part1,
            request.Title3Part2,
            request.Title3Part3,
            request.Title4,
            request.Order
        );

        _repository.Update(slide);
        await _repository.SaveChangesAsync(cancellationToken);

        // Invalidate both caches
        await _cache.RemoveAsync("slides:all", cancellationToken);
        await _cache.RemoveAsync($"slides:{slide.Id}", cancellationToken);

        return new SlideDto(
            slide.Id, slide.ImageUrl, slide.Title1, slide.Title2,
            slide.Title3Part1, slide.Title3Part2, slide.Title3Part3,
            slide.Title4, slide.Order, slide.IsActive, slide.CreatedAt, slide.UpdatedAt
        );
    }
}