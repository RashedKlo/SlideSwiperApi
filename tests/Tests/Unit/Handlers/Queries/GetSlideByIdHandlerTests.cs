using Application.Common.DTOs;
using Application.Common.Interfaces;
using Application.Slides.Queries.GetSlideById;
using Domain.Entities;
using FluentAssertions;
using Moq;
using Tests.TestFixtures;

namespace Tests.Unit.Handlers.Queries;

/// <summary>
/// Unit tests for GetSlideByIdHandler
/// Tests cache behavior and repository lookup by ID
/// </summary>
public class GetSlideByIdHandlerTests
{
    private readonly Mock<ISlideRepository> _mockRepository;
    private readonly Mock<ICacheService> _mockCache;
    private readonly GetSlideByIdHandler _handler;

    public GetSlideByIdHandlerTests()
    {
        _mockRepository = new Mock<ISlideRepository>();
        _mockCache = new Mock<ICacheService>();
        _handler = new GetSlideByIdHandler(_mockRepository.Object, _mockCache.Object);
    }

    [Fact]
    public async Task Handle_WhenCacheIsHit_ShouldReturnCachedSlide()
    {
        // Arrange
        var slideId = Guid.NewGuid();
        var cachedSlide = new SlideDtoBuilder().WithId(slideId).Build();
        var cacheKey = $"slides:{slideId}";

        _mockCache.Setup(c => c.GetAsync<SlideDto>(cacheKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cachedSlide);

        var query = new GetSlideByIdQuery(slideId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(slideId);
        result.Should().BeEquivalentTo(cachedSlide);
    }

    [Fact]
    public async Task Handle_WhenCacheIsHit_ShouldNotCallRepository()
    {
        // Arrange
        var slideId = Guid.NewGuid();
        var cachedSlide = new SlideDtoBuilder().WithId(slideId).Build();
        var cacheKey = $"slides:{slideId}";

        _mockCache.Setup(c => c.GetAsync<SlideDto>(cacheKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cachedSlide);

        var query = new GetSlideByIdQuery(slideId);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _mockRepository.Verify(
            r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenCacheIsMiss_ShouldCallRepository()
    {
        // Arrange
        Guid slideId = Guid.NewGuid();
        var cacheKey = $"slides:{slideId}";
        var slide =   Slide.Create(
                "https://example.com/image.jpg",
               "Title 1",
               "Title 2",
               "Part 1",
               "Part 2",
               null,
               "Title 4",
               1
           );

        _mockCache.Setup(c => c.GetAsync<SlideDto>(cacheKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SlideDto?)null);

        _mockRepository.Setup(r => r.GetByIdAsync(slideId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(slide);

        _mockCache.Setup(c => c.SetAsync(
            cacheKey,
            It.IsAny<SlideDto>(),
            It.IsAny<TimeSpan>(),
            It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var query = new GetSlideByIdQuery(slideId);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _mockRepository.Verify(
            r => r.GetByIdAsync(slideId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenCacheIsMiss_ShouldSetCacheWithData()
    {
        // Arrange
        var slideId = Guid.NewGuid();
        var cacheKey = $"slides:{slideId}";
        var slide =   Slide.Create(
                "https://example.com/image.jpg",
               "Title 1",
               "Title 2",
               "Part 1",
               "Part 2",
               null,
               "Title 4",
               1
           );
        _mockCache.Setup(c => c.GetAsync<SlideDto>(cacheKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SlideDto?)null);

        _mockRepository.Setup(r => r.GetByIdAsync(slideId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(slide);

        _mockCache.Setup(c => c.SetAsync(
            cacheKey,
            It.IsAny<SlideDto>(),
            It.IsAny<TimeSpan>(),
            It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var query = new GetSlideByIdQuery(slideId);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _mockCache.Verify(
            c => c.SetAsync(
                cacheKey,
                It.IsAny<SlideDto>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenSlideNotFound_ShouldReturnNull()
    {
        // Arrange
        var slideId = Guid.NewGuid();
        var cacheKey = $"slides:{slideId}";

        _mockCache.Setup(c => c.GetAsync<SlideDto>(cacheKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SlideDto?)null);

        _mockRepository.Setup(r => r.GetByIdAsync(slideId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Slide?)null);

        var query = new GetSlideByIdQuery(slideId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WhenSlideNotFound_ShouldNotSetCache()
    {
        // Arrange
        var slideId = Guid.NewGuid();
        var cacheKey = $"slides:{slideId}";

        _mockCache.Setup(c => c.GetAsync<SlideDto>(cacheKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SlideDto?)null);

        _mockRepository.Setup(r => r.GetByIdAsync(slideId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Slide?)null);

        var query = new GetSlideByIdQuery(slideId);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _mockCache.Verify(
            c => c.SetAsync(
                cacheKey,
                It.IsAny<SlideDto>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldUseCorrectCacheKey()
    {
        // Arrange
        var slideId = Guid.NewGuid();
        var cacheKey = $"slides:{slideId}";

        _mockCache.Setup(c => c.GetAsync<SlideDto>(cacheKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SlideDto?)null);

        _mockRepository.Setup(r => r.GetByIdAsync(slideId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Slide?)null);

        var query = new GetSlideByIdQuery(slideId);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _mockCache.Verify(
            c => c.GetAsync<SlideDto>(cacheKey, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithValidSlide_ShouldReturnCorrectData()
    {
        // Arrange
        var slideId = Guid.NewGuid();
        var cacheKey = $"slides:{slideId}";
        var slide = Slide.Create(
                "https://example.com/image.jpg",
               "Main Title",
               "Subtitle",
               "Part 1",
               "Part 2",
               "Part 3",
               "Bottom",
               5
           );

        _mockCache.Setup(c => c.GetAsync<SlideDto>(cacheKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SlideDto?)null);

        _mockRepository.Setup(r => r.GetByIdAsync(slideId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(slide);

        _mockCache.Setup(c => c.SetAsync(
            cacheKey,
            It.IsAny<SlideDto>(),
            It.IsAny<TimeSpan>(),
            It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var query = new GetSlideByIdQuery(slideId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.ImageUrl.Should().Be("https://example.com/image.jpg");
        result.Title1.Should().Be("Main Title");
        result.Title2.Should().Be("Subtitle");
        result.Order.Should().Be(5);
        result.IsActive.Should().BeTrue();
    }
}
