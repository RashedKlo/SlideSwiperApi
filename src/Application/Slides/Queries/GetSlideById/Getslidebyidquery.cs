using Application.Common.DTOs;
using MediatR;

namespace Application.Slides.Queries.GetSlideById;

public record GetSlideByIdQuery(Guid Id) : IRequest<SlideDto?>;