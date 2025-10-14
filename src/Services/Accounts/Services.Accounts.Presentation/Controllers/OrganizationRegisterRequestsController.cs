using MediatR;
using Microsoft.AspNetCore.Mvc;
using Services.Accounts.Application.BusinessLogics.OrganizationRegisterRequests.Features.CheckOrganizationRegisterRequest;
using Services.Accounts.Application.BusinessLogics.OrganizationRegisterRequests.Features.CreateOrganizationRegisterRequest;

namespace Services.Accounts.Presentation.Controllers;

[ApiController]
[Route("api/v1/organization-register-requests")]
#pragma warning disable CA1515 // Consider making public types internal
public class OrganizationRegisterRequestsController : BaseApiController
#pragma warning restore CA1515 // Consider making public types internal
{
    private readonly IMediator _mediator;

    public OrganizationRegisterRequestsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateOrganizationRequestCommand command)
    {
        OrganizationRegisterRequestResponseDto result = await _mediator.Send(command);
        return Ok(result); // replace by CreatedAtAction when finished GetById
    }

    [HttpPost("check")]
    public async Task<ActionResult> Check([FromBody] CheckOrganizationRegisterRequestCommand command)
    {
        bool result = await _mediator.Send(command);
        if (result)
        {
            return Ok("Organization Request updated successfully!");
        }
        else
        {
            return BadRequest(" Organization Request updated failed!");
        } 
    }
}
