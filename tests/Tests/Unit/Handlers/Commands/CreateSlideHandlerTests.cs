using Application.Common.DTOs;
using Application.Common.Interfaces;
using Application.Slides.Commands.CreateSlide;
using Domain.Entities;
using FluentAssertions;
using Moq;
using Tests.TestFixtures;

namespace Tests.Unit.Handlers.Commands;

/// <summary>
/// Unit tests for CreateSlideHandler
/// Tests repository interaction, cache invalidation, and DTO mapping
/// </summary>
public class CreateSlideHandlerTests
{
    private readonly Mock<ISlideRepository> _mockRepository;
    private readonly Mock<ICacheService> _mockCache;
    private readonly CreateSlideHandler _handler;

    public CreateSlideHandlerTests()
    {
        _mockRepository = new Mock<ISlideRepository>();
        _mockCache = new Mock<ICacheService>();
        _handler = new CreateSlideHandler(_mockRepository.Object, _mockCache.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldAddSlideToRepository()
    {
        // Arrange
        var command = new CreateSlideCommandBuilder().Build();
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Slide>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mockRepository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _mockCache.Setup(c => c.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockRepository.Verify(
            r => r.AddAsync(It.Is<Slide>(s =>
                s.ImageUrl == command.ImageUrl &&
                s.Title1 == command.Title1 &&
                s.Title2 == command.Title2 &&
                s.Order == command.Order),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldCallSaveChangesOnce()
    {
        // Arrange
        var command = new CreateSlideCommandBuilder().Build();
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Slide>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mockRepository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _mockCache.Setup(c => c.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockRepository.Verify(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldInvalidateCacheKey()
    {
        // Arrange
        var command = new CreateSlideCommandBuilder().Build();
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Slide>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mockRepository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _mockCache.Setup(c => c.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockCache.Verify(
            c => c.RemoveAsync("slides:all", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldReturnSlideDto()
    {
        // Arrange
        var command = new CreateSlideCommandBuilder().Build();
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Slide>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mockRepository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _mockCache.Setup(c => c.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<SlideDto>();
        result.ImageUrl.Should().Be(command.ImageUrl);
        result.Title1.Should().Be(command.Title1);
        result.Title2.Should().Be(command.Title2);
        result.Title3Part1.Should().Be(command.Title3Part1);
        result.Title3Part2.Should().Be(command.Title3Part2);
        result.Title3Part3.Should().Be(command.Title3Part3);
        result.Title4.Should().Be(command.Title4);
        result.Order.Should().Be(command.Order);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ReturnedDtoShouldHaveValidId()
    {
        // Arrange
        var command = new CreateSlideCommandBuilder().Build();
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Slide>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mockRepository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _mockCache.Setup(c => c.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ReturnedDtoShouldHaveCreatedAtTimestamp()
    {
        // Arrange
        var command = new CreateSlideCommandBuilder().Build();
        var beforeCall = DateTime.UtcNow;
        
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Slide>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mockRepository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _mockCache.Setup(c => c.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        var afterCall = DateTime.UtcNow;

        // Assert
        result.CreatedAt.Should().BeOnOrAfter(beforeCall);
        result.CreatedAt.Should().BeOnOrBefore(afterCall);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldSetIsActiveToTrue()
    {
        // Arrange
        var command = new CreateSlideCommandBuilder().Build();
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Slide>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mockRepository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _mockCache.Setup(c => c.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenRepositoryThrowsException_ShouldPropagateException()
    {
        // Arrange
        var command = new CreateSlideCommandBuilder().Build();
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Slide>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithCancellationToken_ShouldPassTokenToRepository()
    {
        // Arrange
        var command = new CreateSlideCommandBuilder().Build();
        var cancellationToken = new CancellationToken();
        
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Slide>(), cancellationToken))
            .Returns(Task.CompletedTask);
        _mockRepository.Setup(r => r.SaveChangesAsync(cancellationToken))
            .ReturnsAsync(1);
        _mockCache.Setup(c => c.RemoveAsync(It.IsAny<string>(), cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, cancellationToken);

        // Assert
        _mockRepository.Verify(
            r => r.AddAsync(It.IsAny<Slide>(), cancellationToken),
            Times.Once);
    }
}
