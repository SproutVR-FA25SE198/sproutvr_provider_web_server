using Common.Application.Helpers;
using MediatR;
using Services.Payments.Application.Helpers;

namespace Services.Payments.Application.BusinessLogics.GetPaymentsList;
public class GetPaymentsListQuery : IRequest<PaginatedResult<PaymentDto>>
{
    public PaymentParams SpecParams { get; set; }
    public GetPaymentsListQuery(PaymentParams specParams)
    {
        SpecParams = specParams;
    }
}
