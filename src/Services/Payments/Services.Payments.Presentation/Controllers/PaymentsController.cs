using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Net.payOS.Types;
using Services.Payments.Application.Abstractions;
using Services.Payments.Application.BusinessLogics.CreatePayment;


namespace Services.Payments.Presentation.Controllers;

// Payment operations controller
[Route("api/[controller]")]
[ApiController]
#pragma warning disable CA1515 // Consider making public types internal
public class PaymentsController : ControllerBase
#pragma warning restore CA1515 // Consider making public types internal
{
    private readonly IPayosPaymentService _payosPaymentService;

    public PaymentsController(IPayosPaymentService payosPaymentService)
    {
        _payosPaymentService = payosPaymentService;
    }

    [HttpPost("create")]
    [AllowAnonymous]
    public async Task<ActionResult<CreatePaymentResult>> CreatePaymentLink([FromBody] CreatePaymentDto paymentRequest)
    {
        CreatePaymentResult result = await _payosPaymentService.CreatePayment(paymentRequest);
        return Ok(result);
    }

    [HttpPost("cancel/{orderId}")]
    [AllowAnonymous]
    public async Task<ActionResult<PaymentLinkInformation>> CreateCancelPaymentLink(int orderCode)
    {
        PaymentLinkInformation result = await _payosPaymentService.CancelPayment(orderCode);
        return Ok(result);
    }
}
