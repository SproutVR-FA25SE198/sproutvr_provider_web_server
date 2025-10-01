using MediatR;
using Microsoft.AspNetCore.Mvc;
using Services.Catalogs.Application.BusinessLogics.Maps.GetMaps;
using Services.Catalogs.Application.BusinessLogics.Maps;
using Services.Catalogs.Application.BusinessLogics.Maps.GetMapById;
using Services.Catalogs.Application.BusinessLogics.Maps.CreateMap;
using Services.Catalogs.Application.BusinessLogics.Maps.UpdateMap;
using Services.Catalogs.Application.BusinessLogics.Maps.DeleteMap;
using Common.Application.Helpers;
using Services.Catalogs.Application.BusinessLogics.Maps.DTOs;

namespace Services.Catalogs.Presentation.Controllers;

[Route("api/catalogs/maps")]
#pragma warning disable CA1515 // Consider making public types internal
public sealed class MapsController(IMediator mediator) : BaseApiController
#pragma warning restore CA1515 // Consider making public types internal
{
    [HttpGet]
    public async Task<IActionResult> GetMaps([FromQuery] MapParams mapParams, CancellationToken cancellationToken)
    {
        var query = new GetMapsQuery(mapParams);
        PaginatedResult<MapDto> result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var query = new GetMapByIdQuery(id);
        MapDto result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMapCommand command, CancellationToken cancellationToken)
    {
        MapDto result = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateMapDto dto, CancellationToken cancellationToken)
    {
        var command = new UpdateMapCommand(dto)
        {
            Id = id
        };
        MapDto result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteMapCommand(id);
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }
}
