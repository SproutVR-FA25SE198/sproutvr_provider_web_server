using MediatR;
using Microsoft.AspNetCore.Mvc;
using Services.Accounts.Application.BusinessLogics.ApplicationUsers.Features.Login;
using Services.Accounts.Application.BusinessLogics.ApplicationUsers.Features.ViewProfile;

namespace Services.Accounts.Presentation.Controllers;
[ApiController]
[Route("api/v1/auth")]
#pragma warning disable CA1515 // Consider making public types internal
public class AuthController : BaseApiController
#pragma warning restore CA1515 // Consider making public types internal
{

    private readonly IMediator _mediator;
    
    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginCommand loginCommand)
    {
        LoginResponseDto result = await _mediator.Send(loginCommand);
        return Ok(result);
    }

    // view profile
    [HttpGet("profile")]
    public async Task<ActionResult> ViewProfile ()
    {
        ApplicationUserDto result = await _mediator.Send(new ViewProfileQuery());
        return Ok(result);
    }

    // reset password
}
