using Application.Common.DTOs;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Slides.Queries.GetAllSlides;

public class GetAllSlidesHandler : IRequestHandler<GetAllSlidesQuery, List<SlideDto>>
{
    private readonly ISlideRepository _repository;
    private readonly ICacheService _cache;
    private const string CacheKey = "slides:all";

    public GetAllSlidesHandler(ISlideRepository repository, ICacheService cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<List<SlideDto>> Handle(GetAllSlidesQuery request, CancellationToken cancellationToken)
    {
        // Try cache first
        var cached = await _cache.GetAsync<List<SlideDto>>(CacheKey, cancellationToken);
        if (cached is not null)
            return cached;

        // Fetch from DB
        var slides = await _repository.GetAllAsync(cancellationToken);

        var dtos = slides.Select(s => new SlideDto(
            s.Id, s.ImageUrl, s.Title1, s.Title2,
            s.Title3Part1, s.Title3Part2, s.Title3Part3,
            s.Title4, s.Order, s.IsActive, s.CreatedAt, s.UpdatedAt
        )).ToList();

        // Store in cache for 10 minutes
        await _cache.SetAsync(CacheKey, dtos, TimeSpan.FromMinutes(10), cancellationToken);

        return dtos;
    }
}