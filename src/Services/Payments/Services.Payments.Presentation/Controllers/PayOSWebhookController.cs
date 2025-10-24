using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Net.payOS;
using Net.payOS.Types;
using Services.Payments.Application.Abstractions;
using Services.Payments.Application.BusinessLogics.ConfirmWebhook;

namespace Services.Payments.Presentation.Controllers;

[Route("api/webhook")]
[ApiController]
#pragma warning disable CA1515 // Consider making public types internal
#pragma warning disable S6960 // Controllers should not have mixed responsibilities
public class PayOSWebhookController : ControllerBase
#pragma warning restore S6960 // Controllers should not have mixed responsibilities
#pragma warning restore CA1515 // Consider making public types internal
{
    private readonly PayOS _payOS;
    private readonly IPayosPaymentService _payosPaymentService;
    private readonly ILogger<PayOSWebhookController> _logger;

    public PayOSWebhookController(PayOS payOS, ILogger<PayOSWebhookController> logger, IPayosPaymentService payosPaymentService)
    {
        _payOS = payOS;
        _logger = logger;
        _payosPaymentService = payosPaymentService;
    }

    [HttpPost("confirm")]
    [AllowAnonymous]
    public async Task<ActionResult> ConfirmWebhook([FromBody] ConfirmWebhookRequest request)
    {
        try
        {
            if (request == null || string.IsNullOrEmpty(request.WebhookUrl))
            {
                throw new ArgumentNullException(nameof(request));
            }

            await _payOS.confirmWebhook(request.WebhookUrl);
            return Ok("Webhook URL is valid");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while confirming webhook");
            throw new InvalidOperationException("Invalid webhook URL");
        }
    }


    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult> ProcessPayment([FromBody] WebhookType body)
    {
        try
        {
            bool isSuccess = await _payosPaymentService.ProcessPayment(body);
            if (isSuccess)
            {
                return Ok("PaymentTransaction processed successfully!");
            }
            else
            {
                return BadRequest("PaymentTransaction failed!");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while processing webhook");
            return BadRequest("Error processing webhook");
        }
    }
}
