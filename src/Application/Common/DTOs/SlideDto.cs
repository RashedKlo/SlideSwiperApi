namespace Application.Common.DTOs;

public record SlideDto(
    Guid Id,
    string ImageUrl,
    string Title1,
    string Title2,
    string Title3Part1,
    string Title3Part2,
    string? Title3Part3,
    string Title4,
    int Order,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);