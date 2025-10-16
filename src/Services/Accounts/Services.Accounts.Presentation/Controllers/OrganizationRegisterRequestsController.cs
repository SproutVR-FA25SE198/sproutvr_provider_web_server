using Common.Application.Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Services.Accounts.Application.BusinessLogics.OrganizationRegisterRequests.Features.CheckOrganizationRegisterRequest;
using Services.Accounts.Application.BusinessLogics.OrganizationRegisterRequests.Features.CreateOrganizationRegisterRequest;
using Services.Accounts.Application.BusinessLogics.OrganizationRegisterRequests.Features.GetOrganizationRegisterRequestById;
using Services.Accounts.Application.BusinessLogics.OrganizationRegisterRequests.Features.GetOrganizationRegisterRequests;
using Services.Accounts.Application.BusinessLogics.OrganizationRegisterRequests.Specifications;

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
        OrganizationRegisterRequestDto result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
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

    [HttpGet]
    public async Task<ActionResult<PaginatedResult<OrganizationRegisterRequestDto>>> GetAll([FromQuery] OrganizationRequestSpecParams specParams)
    {
        var query = new GetOrganizationRegisterRequestsQuery(specParams);
        PaginatedResult<OrganizationRegisterRequestDto> paginatedResult = await _mediator.Send(query);
        return Ok(paginatedResult);
    }

    [HttpGet("{id:Guid}")]
    public async Task<ActionResult<OrganizationRegisterRequestDetailsDto>> GetById([FromRoute] Guid id)
    {
        OrganizationRegisterRequestDetailsDto result = await _mediator.Send(new GetOrganizationRegisterRequestByIdQuery(id));
        return Ok(result);
    }
}
