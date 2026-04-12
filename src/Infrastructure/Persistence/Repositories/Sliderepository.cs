using Application.Common.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class SlideRepository : ISlideRepository
{
    private readonly AppDbContext _context;

    public SlideRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Slide>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Slides
            .Where(s => s.IsActive)
            .OrderBy(s => s.Order)
            .ToListAsync(cancellationToken);
    }

    public async Task<Slide?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Slides
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task AddAsync(Slide slide, CancellationToken cancellationToken = default)
    {
        await _context.Slides.AddAsync(slide, cancellationToken);
    }

    public void Update(Slide slide)
    {
        _context.Slides.Update(slide);
    }

    public void Delete(Slide slide)
    {
        _context.Slides.Remove(slide);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}