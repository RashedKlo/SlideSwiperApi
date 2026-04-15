using Application.Slides.Commands.CreateSlide;
using Application.Slides.Commands.UpdateSlide;

namespace Tests.TestFixtures;

/// <summary>
/// Builder for creating valid CreateSlideCommand instances for testing
/// </summary>
public class CreateSlideCommandBuilder
{
    private string _imageUrl = "https://example.com/image.jpg";
    private string _title1 = "Main Title";
    private string _title2 = "Subtitle";
    private string _title3Part1 = "Part 1";
    private string _title3Part2 = "Part 2";
    private string? _title3Part3 = "Part 3";
    private string _title4 = "Bottom Text";
    private int _order = 1;

    public CreateSlideCommand Build() => new(
        ImageUrl: _imageUrl,
        Title1: _title1,
        Title2: _title2,
        Title3Part1: _title3Part1,
        Title3Part2: _title3Part2,
        Title3Part3: _title3Part3,
        Title4: _title4,
        Order: _order
    );

    public CreateSlideCommandBuilder WithImageUrl(string imageUrl)
    {
        _imageUrl = imageUrl;
        return this;
    }

    public CreateSlideCommandBuilder WithTitle1(string title1)
    {
        _title1 = title1;
        return this;
    }

    public CreateSlideCommandBuilder WithTitle2(string title2)
    {
        _title2 = title2;
        return this;
    }

    public CreateSlideCommandBuilder WithTitle3(string part1, string part2, string? part3 = null)
    {
        _title3Part1 = part1;
        _title3Part2 = part2;
        _title3Part3 = part3;
        return this;
    }

    public CreateSlideCommandBuilder WithTitle4(string title4)
    {
        _title4 = title4;
        return this;
    }

    public CreateSlideCommandBuilder WithOrder(int order)
    {
        _order = order;
        return this;
    }
}

/// <summary>
/// Builder for creating valid UpdateSlideCommand instances for testing
/// </summary>
public class UpdateSlideCommandBuilder
{
    private Guid _id = Guid.NewGuid();
    private string _imageUrl = "https://example.com/image.jpg";
    private string _title1 = "Updated Title";
    private string _title2 = "Updated Subtitle";
    private string _title3Part1 = "Updated Part 1";
    private string _title3Part2 = "Updated Part 2";
    private string? _title3Part3 = "Updated Part 3";
    private string _title4 = "Updated Bottom Text";
    private int _order = 2;

    public UpdateSlideCommand Build() => new(
        Id: _id,
        ImageUrl: _imageUrl,
        Title1: _title1,
        Title2: _title2,
        Title3Part1: _title3Part1,
        Title3Part2: _title3Part2,
        Title3Part3: _title3Part3,
        Title4: _title4,
        Order: _order
    );

    public UpdateSlideCommandBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public UpdateSlideCommandBuilder WithImageUrl(string imageUrl)
    {
        _imageUrl = imageUrl;
        return this;
    }

    public UpdateSlideCommandBuilder WithAllFields(
        string imageUrl = "https://example.com/image.jpg",
        string title1 = "Title1",
        string title2 = "Title2",
        string title3Part1 = "Title3Part1",
        string title3Part2 = "Title3Part2",
        string title4 = "Title4",
        int order = 1)
    {
        _imageUrl = imageUrl;
        _title1 = title1;
        _title2 = title2;
        _title3Part1 = title3Part1;
        _title3Part2 = title3Part2;
        _title4 = title4;
        _order = order;
        return this;
    }
}
