using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Net.payOS;
using Net.payOS.Types;
using Services.Payments.Application.BusinessLogics.ConfirmWebhook;
using Services.Payments.Domain.Entities.Payments;

namespace Services.Payments.Presentation.Controllers;

[Route("api/webhook")]
[ApiController]
#pragma warning disable CA1515 // Consider making public types internal
public class PayOSWebhookController : ControllerBase
#pragma warning restore CA1515 // Consider making public types internal
{
    private readonly PayOS _payOS;
    private readonly ILogger<PayOSWebhookController> _logger;

    public PayOSWebhookController(PayOS payOS, ILogger<PayOSWebhookController> logger)
    {
        _payOS = payOS;
        _logger = logger;
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
    #pragma warning disable S6968 // Actions that return a value should be annotated with ProducesResponseTypeAttribute containing the return type
    public ActionResult ProcessPayment([FromBody] WebhookType body)
    #pragma warning restore S6968 // Actions that return a value should be annotated with ProducesResponseTypeAttribute containing the return type
    {
        try
        {
            ArgumentNullException.ThrowIfNull(body);

            WebhookData data = _payOS.verifyPaymentWebhookData(body);

            // For setting up webhook only
            if (data.description == "Ma giao dich thu nghiem" || data.description == "VQRIO123")
            {
                return Ok();
            }

            // Check the status code
            OrderStatus orderStatus = data.code == "00" ? OrderStatus.Pending_Bundle : OrderStatus.Payment_Failed;

            _logger.LogInformation("Webhook received for order #{OrderCode} with status {OrderStatus}", data.orderCode, orderStatus.ToString());

            //-----------------
            // RabbitMQ to publish event to update order status in Orders service
            //-----------------

            //-----------------
            return Ok("Updated order");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while processing payment webhook");
            throw new InvalidOperationException("Error while processing payment");
        }
    }
}
