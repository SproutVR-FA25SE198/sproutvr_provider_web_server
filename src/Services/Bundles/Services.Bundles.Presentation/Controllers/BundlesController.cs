using MediatR;
using Microsoft.AspNetCore.Mvc;
using Services.Bundles.Application.BusinessLogics.UploadBundle;

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

}
