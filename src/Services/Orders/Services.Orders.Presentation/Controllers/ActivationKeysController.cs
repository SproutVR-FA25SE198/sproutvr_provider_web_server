using Microsoft.AspNetCore.Mvc;
using Services.Orders.Application.Abstractions.Services;

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

    public ActivationKeysController(IActivationKeyGeneratorService activationKeyGeneratorService)
    {
        _activationKeyGeneratorService = activationKeyGeneratorService;
    }

    /// <summary>
    /// For testing only, generate a key
    /// </summary>
    [HttpPost]
    public IActionResult GenerateActivationKey()
    {
        string activationKey = _activationKeyGeneratorService.Generate();
        return Ok(activationKey);
    }
}
