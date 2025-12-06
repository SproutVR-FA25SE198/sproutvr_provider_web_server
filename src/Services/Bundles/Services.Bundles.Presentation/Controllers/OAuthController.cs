using Microsoft.AspNetCore.Mvc;
using Services.Bundles.Infrastructure.Helpers;

namespace Services.Bundles.Presentation.Controllers;

[Route("[controller]")]
[ApiController]
#pragma warning disable CA1515 // Consider making public types internal
public class OAuthController : ControllerBase
#pragma warning restore CA1515 // Consider making public types internal
{
    [HttpGet("login")]
    public IActionResult Login([FromServices] OAuthHelper oauth)
    {
        return Redirect(oauth.GetGoogleOAuthUrl());
    }

    [HttpGet("callback")]
    public async Task<IActionResult> Callback([FromQuery] string code, [FromServices] OAuthHelper oauth)
    {
        await oauth.ExchangeCodeForRefreshToken(code);
        return Content("Refresh token saved");
    }

}
