using Application.Common.Interfaces;
using MediatR;

namespace Application.Slides.Commands.DeleteSlide;

public class DeleteSlideHandler : IRequestHandler<DeleteSlideCommand, bool>
{
    private readonly ISlideRepository _repository;
    private readonly ICacheService _cache;

    public DeleteSlideHandler(ISlideRepository repository, ICacheService cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<bool> Handle(DeleteSlideCommand request, CancellationToken cancellationToken)
    {
        var slide = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (slide is null) return false;

        _repository.Delete(slide);
        await _repository.SaveChangesAsync(cancellationToken);

        // Invalidate caches
        await _cache.RemoveAsync("slides:all", cancellationToken);
        await _cache.RemoveAsync($"slides:{request.Id}", cancellationToken);

        return true;
    }
}