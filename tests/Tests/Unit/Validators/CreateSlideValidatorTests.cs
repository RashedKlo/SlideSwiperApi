using Application.Slides.Commands.CreateSlide;
using FluentAssertions;
using FluentValidation;
using Tests.TestFixtures;

namespace Tests.Unit.Validators;

/// <summary>
/// Unit tests for CreateSlideValidator
/// Tests that validation rules are correctly enforced for required fields
/// </summary>
public class CreateSlideValidatorTests
{
    private readonly CreateSlideValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_ShouldPass()
    {
        // Arrange
        var command = new CreateSlideCommandBuilder().Build();

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_WithEmptyImageUrl_ShouldFail()
    {
        // Arrange
        var command = new CreateSlideCommandBuilder()
            .WithImageUrl(string.Empty)
            .Build();

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "ImageUrl");
    }

    [Fact]
    public void Validate_WithEmptyTitle1_ShouldFail()
    {
        // Arrange
        var command = new CreateSlideCommandBuilder()
            .WithTitle1(string.Empty)
            .Build();

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Title1");
    }

    [Fact]
    public void Validate_WithEmptyTitle2_ShouldFail()
    {
        // Arrange
        var command = new CreateSlideCommandBuilder()
            .WithTitle2(string.Empty)
            .Build();

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Title2");
    }

    [Fact]
    public void Validate_WithEmptyTitle3Part1_ShouldFail()
    {
        // Arrange
        var command = new CreateSlideCommandBuilder()
            .WithTitle3(string.Empty, "Part 2")
            .Build();

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Title3Part1");
    }

    [Fact]
    public void Validate_WithEmptyTitle3Part2_ShouldFail()
    {
        // Arrange
        var command = new CreateSlideCommandBuilder()
            .WithTitle3("Part 1", string.Empty)
            .Build();

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Title3Part2");
    }

    [Fact]
    public void Validate_WithEmptyTitle4_ShouldFail()
    {
        // Arrange
        var command = new CreateSlideCommandBuilder()
            .WithTitle4(string.Empty)
            .Build();

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Title4");
    }

    [Fact]
    public void Validate_WithNullTitle3Part3_ShouldPass()
    {
        // Arrange - Title3Part3 is optional
        var command = new CreateSlideCommandBuilder()
            .WithTitle3("Part1", "Part2", null)
            .Build();

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithNegativeOrder_ShouldFail()
    {
        // Arrange
        var command = new CreateSlideCommandBuilder()
            .WithOrder(-1)
            .Build();

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Order");
    }

    [Fact]
    public void Validate_WithZeroOrder_ShouldPass()
    {
        // Arrange
        var command = new CreateSlideCommandBuilder()
            .WithOrder(0)
            .Build();

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithImageUrlExceedingMaxLength_ShouldFail()
    {
        // Arrange
        var longImageUrl = new string('a', 501);
        var command = new CreateSlideCommandBuilder()
            .WithImageUrl(longImageUrl)
            .Build();

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "ImageUrl");
    }

    [Fact]
    public void Validate_WithTitle1ExceedingMaxLength_ShouldFail()
    {
        // Arrange
        var longTitle = new string('a', 201);
        var command = new CreateSlideCommandBuilder()
            .WithTitle1(longTitle)
            .Build();

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Title1");
    }

    [Fact]
    public void Validate_WithMultipleErrors_ShouldReturnAllErrors()
    {
        // Arrange
        var command = new CreateSlideCommandBuilder()
            .WithTitle1(string.Empty)
            .WithTitle2(string.Empty)
            .WithOrder(-5)
            .Build();

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(3);
    }
}
