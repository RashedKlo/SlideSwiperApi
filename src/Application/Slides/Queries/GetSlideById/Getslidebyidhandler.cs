using Application.Common.DTOs;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Slides.Queries.GetSlideById;

public class GetSlideByIdHandler : IRequestHandler<GetSlideByIdQuery, SlideDto?>
{
    private readonly ISlideRepository _repository;
    private readonly ICacheService _cache;

    public GetSlideByIdHandler(ISlideRepository repository, ICacheService cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<SlideDto?> Handle(GetSlideByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"slides:{request.Id}";

        var cached = await _cache.GetAsync<SlideDto>(cacheKey, cancellationToken);
        if (cached is not null)
            return cached;

        var slide = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (slide is null) return null;

        var dto = new SlideDto(
            slide.Id, slide.ImageUrl, slide.Title1, slide.Title2,
            slide.Title3Part1, slide.Title3Part2, slide.Title3Part3,
            slide.Title4, slide.Order, slide.IsActive, slide.CreatedAt, slide.UpdatedAt
        );

        await _cache.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(10), cancellationToken);

        return dto;
    }
}