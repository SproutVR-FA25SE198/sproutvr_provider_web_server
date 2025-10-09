using Grpc.Core;
using Net.payOS.Types;
using PaymentsService;
using Services.Payments.Application.Abstractions;
using Services.Payments.Application.BusinessLogics.CreatePayment;
using Services.Payments.Domain.Entities.Payments;

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
        PaymentMethod paymentMethod = Enum.Parse<PaymentMethod>(request.PaymentMethod);
        var response = new CreatePaymentResponse();

        // Payos
        if (paymentMethod == PaymentMethod.PAYOS)
        {
            CreatePaymentResult paymentResult = await _payosPaymentService.CreatePayment
                (
                new CreatePaymentDto
                {
                    OrderCode = request.OrderCode,
                    TotalMoneyAmount = request.TotalMoneyAmount
                }
            );
            response.PaymentUrl = paymentResult.checkoutUrl;
        }

        return response;
    }
}
