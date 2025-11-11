using MediatR;
using Microsoft.AspNetCore.Mvc;
using Services.Orders.Application.BusinessLogics.ActivationKeys.Features.GetBundles;
using Services.Orders.Application.BusinessLogics.ActivationKeys.Features.MarkAsDownloaded;

namespace Services.Orders.Presentation.Controllers;

[ApiController]
[Route("api/bundle-payloads")]
#pragma warning disable CA1515 // Consider making public types internal
public class BundlePayloadsController : BaseApiController
{
    private readonly IMediator _mediator;

    public BundlePayloadsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get activated bundle
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> GetActivatedBundles([FromBody] GetBundlesRequestDto requestDto, CancellationToken cancellationToken)
    {
        var query = new GetBundlesQuery(requestDto);
        List<BundlePayloadDto> result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get activated bundle by id
    /// </summary>
    [HttpPost("{id:guid}")]
    public async Task<IActionResult> GetActivatedBundleById([FromRoute] Guid id, [FromBody] GetBundleByIdRequestDto requestDto, CancellationToken cancellationToken)
    {
        var query = new GetBundleByIdQuery(id, requestDto);
        BundlePayloadDto result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Mark order item as downloaded
    /// </summary>
    [HttpPatch("items/{id:guid}")]
    public async Task<IActionResult> MarkItemAsDownloaded([FromRoute] Guid id, [FromBody] MarkAsDownloadedRequestDto requestDto, CancellationToken cancellationToken)
    {
        var command = new MarkAsDownloadCommand(id, requestDto);
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }
}
