using MediatR;
using Microsoft.AspNetCore.Mvc;
using Common.Application.Helpers;
using Services.Catalogs.Application.BusinessLogics.Maps.Specifications;
using Services.Catalogs.Application.BusinessLogics.Maps.Features.CreateMap;
using Services.Catalogs.Application.BusinessLogics.Maps.Features.GetMapById;
using Services.Catalogs.Application.BusinessLogics.Maps.Features.GetMaps;
using Services.Catalogs.Application.BusinessLogics.Maps.Features.UpdateMap;
using Services.Catalogs.Application.BusinessLogics.Maps.Features.DeleteMap;
using Services.Catalogs.Application.BusinessLogics.Maps.Features.GetMapsByIds;
using Microsoft.AspNetCore.Authorization;
using Common.Domain;

namespace Services.Catalogs.Presentation.Controllers;

[ApiController]
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
        MapDetailsDto result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost("list")]
    public async Task<IActionResult> GetByIds([FromBody] List<string> ids, CancellationToken cancellationToken)
    {
        var query = new GetMapsByIdsQuery(ids);
        List<MapDto> result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = CommonAppCts.Roles.SystemAdmin)]
    public async Task<IActionResult> Create([FromBody] CreateMapDto dto, CancellationToken cancellationToken)
    {
        var command = new CreateMapCommand(dto);
        MapDto result = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = CommonAppCts.Roles.SystemAdmin)]
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
    [Authorize(Roles = CommonAppCts.Roles.SystemAdmin)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteMapCommand(id);
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }
}
