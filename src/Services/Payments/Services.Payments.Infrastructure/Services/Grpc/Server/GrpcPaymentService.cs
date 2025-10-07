using Grpc.Core;
using Net.payOS.Types;
using PaymentsService;
using Services.Payments.Application.Abstractions;
using Services.Payments.Application.BusinessLogics.CreatePayment;

namespace Services.Payments.Infrastructure.Services.Grpc.Server;
public class GrpcPaymentService : GrpcPayment.GrpcPaymentBase
{
    private readonly IPayosPaymentService _payosPaymentService;

    public GrpcPaymentService(IPayosPaymentService payosPaymentService)
    {
        _payosPaymentService = payosPaymentService;
    }

    public override async Task<CreatePaymentResponse> CreatePayment(CreatePaymentRequest request, ServerCallContext context)
    {
        CreatePaymentResult paymentUrl = await _payosPaymentService.CreatePayment(new CreatePaymentDto { OrderCode = request.OrderCode, TotalMoneyAmount = request.TotalMoneyAmount });
        var response = new CreatePaymentResponse
        {
            PaymentUrl = paymentUrl.checkoutUrl
        };
        return response;
    }
}
