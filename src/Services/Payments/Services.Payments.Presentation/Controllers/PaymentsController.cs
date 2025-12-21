using Common.Application.Helpers;
using Common.Domain;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Net.payOS;
using Net.payOS.Types;
using Services.Payments.Application.Abstractions;
using Services.Payments.Application.BusinessLogics.ConfirmWebhook;
using Services.Payments.Application.BusinessLogics.CreatePayment;
using Services.Payments.Application.BusinessLogics.GetPaymentsList;
using Services.Payments.Application.Helpers;


namespace Services.Payments.Presentation.Controllers;

// PaymentTransaction operations controller
[Route("api/[controller]")]
[ApiController]
#pragma warning disable CA1515 // Consider making public types internal
#pragma warning disable S6960 // Controllers should not have mixed responsibilities
public class PaymentsController : ControllerBase
#pragma warning restore S6960 // Controllers should not have mixed responsibilities
#pragma warning restore CA1515 // Consider making public types internal
{
    private readonly PayOS _payOS;
    private readonly IPayosPaymentService _payosPaymentService;
    private readonly ILogger<PaymentsController> _logger;
    private readonly IMediator _mediator;

    public PaymentsController(IPayosPaymentService payosPaymentService, PayOS payOS, ILogger<PaymentsController> logger, IMediator mediator)
    {
        _payosPaymentService = payosPaymentService;
        _payOS = payOS;
        _logger = logger;
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResult<PaymentDto>>> GetPaymentsList([FromQuery] PaymentParams paymentParams, CancellationToken cancellationToken)
    {
        var query = new GetPaymentsListQuery(paymentParams);
        PaginatedResult<PaymentDto> result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost("create")]
    [AllowAnonymous]
    public async Task<ActionResult<CreatePaymentResult>> CreatePaymentLink([FromBody] CreatePaymentDto paymentRequest)
    {
        CreatePaymentResult result = await _payosPaymentService.CreatePayment(paymentRequest);
        return Ok(result);
    }

    [HttpPost("cancel/{orderCode}")]
    [AllowAnonymous]
    public async Task<ActionResult<PaymentLinkInformation>> CancelPaymentLink([FromRoute] long orderCode)
    {
        PaymentLinkInformation result = await _payosPaymentService.CancelPayment(orderCode);
        return Ok(result);
    }

    [HttpPost("webhook/confirm")]
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


    [HttpPost("webhook")]
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
