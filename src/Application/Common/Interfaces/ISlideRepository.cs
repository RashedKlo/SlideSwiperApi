using Domain.Entities;

namespace Application.Common.Interfaces;

public interface ISlideRepository
{
    Task<List<Slide>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Slide?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Slide slide, CancellationToken cancellationToken = default);
    void Update(Slide slide);
    void Delete(Slide slide);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}