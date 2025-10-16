using Common.Application.Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Services.Accounts.Application.BusinessLogics.Organizations.Features.CreateOrganization;
using Services.Accounts.Application.BusinessLogics.Organizations.Features.GetOrganizationById;
using Services.Accounts.Application.BusinessLogics.Organizations.Features.GetOrganizations;
using Services.Accounts.Application.BusinessLogics.Organizations.Features.UpdateOrganization;

namespace Services.Accounts.Presentation.Controllers;
[Route("api/v1/organizations")]
[ApiController]
#pragma warning disable CA1515 // Consider making public types internal
public class OrganizationsController(IMediator mediator) : ControllerBase
#pragma warning restore CA1515 // Consider making public types internal
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetOrganizationsDto dto)
    {
        PaginatedResult<OrganizationDto> result = await mediator.Send(new GetOrganizationsQuery(dto));
        return Ok(result);
    }

    [HttpGet("{id:Guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        OrganizationDetailsDto result = await mediator.Send(new GetOrganizationByIdQuery(id));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateOrganizationCommand request)
    {
        await mediator.Send(request);
        return Ok("Created successfully!"); // replace by CreatedAtAction when finished get by email
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute]Guid id, UpdateOrganizationCommand request)
    {
        request.Id = id;
        await mediator.Send(request);
        return Ok("Updated successfully!");
    }

    // update org info (for org)
    // (mac address (once) and avatar url)

    // deactivate org
    
}
