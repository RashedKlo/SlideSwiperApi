using Application.Slides.Commands.CreateSlide;
using Application.Slides.Commands.DeleteSlide;
using Application.Slides.Commands.UpdateSlide;
using Application.Slides.Queries.GetAllSlides;
using Application.Slides.Queries.GetSlideById;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Api.Controllers;

[ApiController]
[Route("api/slides")]
[EnableRateLimiting("fixed")]
public class SlidesController : ControllerBase
{
    private readonly IMediator _mediator;

    public SlidesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Get all active slides ordered by Order field</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllSlidesQuery(), cancellationToken);
        return Ok(result);
    }

    /// <summary>Get a single slide by ID</summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetSlideByIdQuery(id), cancellationToken);
        if (result is null) return NotFound($"Slide with ID {id} was not found.");
        return Ok(result);
    }

    /// <summary>Create a new slide</summary>
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateSlideCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>Update an existing slide</summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateSlideCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest("ID in URL does not match ID in body.");

        var result = await _mediator.Send(command, cancellationToken);
        if (result is null) return NotFound($"Slide with ID {id} was not found.");
        return Ok(result);
    }

    /// <summary>Delete a slide</summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _mediator.Send(new DeleteSlideCommand(id), cancellationToken);
        if (!deleted) return NotFound($"Slide with ID {id} was not found.");
        return NoContent();
    }
}