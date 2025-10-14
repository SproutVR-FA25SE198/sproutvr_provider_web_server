using MediatR;
using Microsoft.AspNetCore.Mvc;
using Services.Accounts.Application.BusinessLogics.Organizations.Features.CreateOrganization;
using Services.Accounts.Application.BusinessLogics.Organizations.Features.UpdateOrganization;

namespace Services.Accounts.Presentation.Controllers;
[Route("api/v1/organizations")]
[ApiController]
#pragma warning disable CA1515 // Consider making public types internal
public class OrganizationsController(IMediator mediator) : ControllerBase
#pragma warning restore CA1515 // Consider making public types internal
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateOrganizationCommand request)
    {
        await mediator.Send(request);
        return Ok("Created successfully!"); // replace by CreatedAtAction when finished GetById
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute]Guid id, UpdateOrganizationCommand request)
    {
        request.Id = id;
        await mediator.Send(request);
        return Ok("Updated successfully!");
    }
}
