using Application.Common.DTOs;
using MediatR;

namespace Application.Slides.Queries.GetAllSlides;

public record GetAllSlidesQuery() : IRequest<List<SlideDto>>;


 