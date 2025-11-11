using Common.Domain;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Orders.Application.Abstractions.Services;
using Services.Orders.Application.BusinessLogics.ActivationKeys.GetBundles;
using Services.Orders.Application.BusinessLogics.ActivationKeys.ValidateActivationKey;

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
        bool result = await _mediator.Send(command, cancellationToken);

        // Successful activation
        if (result)
        {
            return Ok(new
            {
                StatusCode = 200,
                Message = "Đã kích hoạt học liệu thành công"
            });
        }
        
        // Unexpected error
        return BadRequest(
        new
        {
            StatusCode = 400,
            Message = "Quá trình kích hoạt gặp lỗi không xác định, vui lòng thử lại sau"
        });
    }
}
