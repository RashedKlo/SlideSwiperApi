using Application.Slides.Commands.UpdateSlide;
using FluentAssertions;
using Tests.TestFixtures;

namespace Tests.Unit.Validators;

/// <summary>
/// Unit tests for UpdateSlideValidator
/// Tests that validation rules are correctly enforced including Id validation
/// </summary>
public class UpdateSlideValidatorTests
{
    private readonly UpdateSlideValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_ShouldPass()
    {
        // Arrange
        var command = new UpdateSlideCommandBuilder().Build();

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_WithEmptyId_ShouldFail()
    {
        // Arrange
        var command = new UpdateSlideCommandBuilder()
            .WithId(Guid.Empty)
            .Build();

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Id");
    }

    [Fact]
    public void Validate_WithValidId_ShouldPass()
    {
        // Arrange
        var validId = Guid.NewGuid();
        var command = new UpdateSlideCommandBuilder()
            .WithId(validId)
            .Build();

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
