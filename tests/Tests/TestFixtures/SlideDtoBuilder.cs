using Application.Common.DTOs;

namespace Tests.TestFixtures;

/// <summary>
/// Builder for creating SlideDto instances for testing
/// </summary>
public class SlideDtoBuilder
{
    private Guid _id = Guid.NewGuid();
    private string _imageUrl = "https://example.com/image.jpg";
    private string _title1 = "Main Title";
    private string _title2 = "Subtitle";
    private string _title3Part1 = "Part 1";
    private string _title3Part2 = "Part 2";
    private string? _title3Part3 = "Part 3";
    private string _title4 = "Bottom Text";
    private int _order = 1;
    private bool _isActive = true;
    private DateTime _createdAt = DateTime.UtcNow;
    private DateTime? _updatedAt = null;

    public SlideDto Build() => new(
        Id: _id,
        ImageUrl: _imageUrl,
        Title1: _title1,
        Title2: _title2,
        Title3Part1: _title3Part1,
        Title3Part2: _title3Part2,
        Title3Part3: _title3Part3,
        Title4: _title4,
        Order: _order,
        IsActive: _isActive,
        CreatedAt: _createdAt,
        UpdatedAt: _updatedAt
    );

    public SlideDtoBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public SlideDtoBuilder WithOrder(int order)
    {
        _order = order;
        return this;
    }

    public SlideDtoBuilder WithMultiple(int count)
    {
        var list = new List<SlideDto>();
        for (int i = 0; i < count; i++)
        {
            list.Add(new SlideDtoBuilder()
                .WithId(Guid.NewGuid())
                .WithOrder(i)
                .Build());
        }
        return this;
    }

    public SlideDtoBuilder WithIsActive(bool isActive)
    {
        _isActive = isActive;
        return this;
    }
}
