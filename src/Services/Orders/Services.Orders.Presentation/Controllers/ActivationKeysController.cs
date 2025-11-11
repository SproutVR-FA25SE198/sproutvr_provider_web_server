using MediatR;
using Microsoft.AspNetCore.Mvc;
using Services.Orders.Application.Abstractions.Services;
using Services.Orders.Application.BusinessLogics.ActivationKeys.Features.ValidateActivationKey;

namespace Services.Orders.Presentation.Controllers;

/// <summary>
/// For generation and validation of activation key
/// </summary>
[ApiController]
[Route("api/keys")]
#pragma warning disable CA1515 // Consider making public types internal
public class ActivationKeysController : BaseApiController
{
    private readonly IActivationKeyGeneratorService _activationKeyGeneratorService;
    private readonly IMediator _mediator;

    public ActivationKeysController(IActivationKeyGeneratorService activationKeyGeneratorService, 
        IMediator mediator)
    {
        _activationKeyGeneratorService = activationKeyGeneratorService;
        _mediator = mediator;
    }

    /// <summary>
    /// For testing only, generate a key
    /// </summary>
    [HttpPost("generate")]
    public IActionResult GenerateActivationKey()
    {
        string activationKey = _activationKeyGeneratorService.Generate();
        return Ok(activationKey);
    }

    /// <summary>
    /// Key validation
    /// </summary>
    [HttpPost("validate")]
    public async Task<IActionResult> ValidateActivationKey([FromBody] ActivationRequestDto requestDto, CancellationToken cancellationToken)
    {
        var command = new ValidateActivationKeyCommand(requestDto);
        ValidateActivationKeyPayloadDto result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}
