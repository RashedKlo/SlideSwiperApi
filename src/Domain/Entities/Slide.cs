namespace Domain.Entities;

public class Slide
{
    public Guid Id { get; private set; }
    public string ImageUrl { get; private set; } = string.Empty;
    public string Title1 { get; private set; } = string.Empty;  // top text
    public string Title2 { get; private set; } = string.Empty;  // middle text
    public string Title3Part1 { get; private set; } = string.Empty;  // normal text
    public string Title3Part2 { get; private set; } = string.Empty;  // highlighted red
    public string? Title3Part3 { get; private set; }                 // highlighted bold (optional)
    public string Title4 { get; private set; } = string.Empty;  // bottom text
    public int Order { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // EF Core needs this
    private Slide() { }

    public static Slide Create(
        string imageUrl,
        string title1,
        string title2,
        string title3Part1,
        string title3Part2,
        string? title3Part3,
        string title4,
        int order)
    {
        return new Slide
        {
            Id = Guid.NewGuid(),
            ImageUrl = imageUrl,
            Title1 = title1,
            Title2 = title2,
            Title3Part1 = title3Part1,
            Title3Part2 = title3Part2,
            Title3Part3 = title3Part3,
            Title4 = title4,
            Order = order,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(
        string imageUrl,
        string title1,
        string title2,
        string title3Part1,
        string title3Part2,
        string? title3Part3,
        string title4,
        int order)
    {
        ImageUrl = imageUrl;
        Title1 = title1;
        Title2 = title2;
        Title3Part1 = title3Part1;
        Title3Part2 = title3Part2;
        Title3Part3 = title3Part3;
        Title4 = title4;
        Order = order;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate() => IsActive = false;
}