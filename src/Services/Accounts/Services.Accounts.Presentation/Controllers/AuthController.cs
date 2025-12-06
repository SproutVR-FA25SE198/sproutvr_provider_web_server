using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Services.Accounts.Application.BusinessLogics.ApplicationUsers.Features.ChangePassword;
using Services.Accounts.Application.BusinessLogics.ApplicationUsers.Features.Login;
using Services.Accounts.Application.BusinessLogics.ApplicationUsers.Features.ViewProfile;
using Services.Accounts.Application.BusinessLogics.Organizations.Features.UpdateOrganizationProfile;

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

    // create reset password link

    // verify reset password link

    // change password
    [HttpPost("change-password")]
    public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordCommand changePasswordCommand)
    {
        IdentityResult result = await _mediator.Send(changePasswordCommand);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return Ok(new { message = "Password changed successfully." });
    }

    // update organization profile
    [HttpPut("organization/profile")]
    public async Task<ActionResult> UpdateOrganizationProfile([FromBody] UpdateOrganizationProfileCommand updateCommand)
    {
        bool result = await _mediator.Send(updateCommand);
        if (!result)
        {
            return BadRequest(new { message = "Failed to update organization profile!" });
        }
        return Ok(new { message = "Organization profile updated successfully." });
    }
}
