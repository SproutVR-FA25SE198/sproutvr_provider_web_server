using Services.Payments.Application.BusinessLogics.CreatePayment;
using Net.payOS.Types;

namespace Services.Payments.Application.Abstractions;
public interface IPayosPaymentService
{
    Task<CreatePaymentResult> CreatePayment(CreatePaymentDto dto);
    Task<bool> VerifyPayment(int orderCode);
    Task<PaymentLinkInformation> CancelPayment(int orderCode);
}
