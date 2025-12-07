using MediatR;
using Microsoft.AspNetCore.Mvc;
using Services.Bundles.Application.BusinessLogics.UploadBundle;
using Services.Bundles.Infrastructure.Helpers;

namespace Services.Bundles.Presentation.Controllers;
[Route("api/[controller]")]
[ApiController]
#pragma warning disable CA1515 // Consider making public types internal
public class BundlesController(IMediator mediator) : ControllerBase
#pragma warning restore CA1515 // Consider making public types internal
{
    // upload bundle
    [HttpPost("upload")]
    [RequestSizeLimit(2147483648)] // 2 GB
    [RequestFormLimits(MultipartBodyLengthLimit = 2147483648)]
    public async Task<IActionResult> UploadBundle(IFormFile bundleFile, [FromForm]OrderDto orderDto, CancellationToken cancellationToken)
    {
        var command = new UploadBundleCommand { BundleFile = bundleFile, OrderDto = orderDto };
        await mediator.Send(command, cancellationToken);
        return Ok(new { message = "Bundle uploaded successfully" });
    }

    [HttpGet("oauth/login")]
    public IActionResult Login([FromServices] OAuthHelper oauth)
    {
        return Redirect(oauth.GetGoogleOAuthUrl());
    }

    [HttpGet("oauth/callback")]
    public async Task<IActionResult> Callback([FromQuery] string code, [FromServices] OAuthHelper oauth)
    {
        await oauth.ExchangeCodeForRefreshToken(code);
        return Content("Refresh token saved");
    }

}
