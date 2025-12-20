using Common.Application.Helpers;
using Services.Payments.Domain.Entities.Payments;

namespace Services.Payments.Application.Helpers;
public class PaymentSpecification : BaseSpecification<PaymentTransaction>
{
    public PaymentSpecification(PaymentParams paymentParams) : base(p =>
        (!paymentParams.MinAmount.HasValue || p.Amount >= paymentParams.MinAmount) &&
        (!paymentParams.MaxAmount.HasValue || p.Amount <= paymentParams.MaxAmount) &&
        (!paymentParams.OrderId.HasValue || p.OrderId == paymentParams.OrderId) && 
        (string.IsNullOrEmpty(paymentParams.Status) || p.Status.ToString() == paymentParams.Status) &&
        (!paymentParams.FromDate.HasValue || p.CreatedAtUtc >= paymentParams.FromDate) &&
        (!paymentParams.ToDate.HasValue || p.CreatedAtUtc <= paymentParams.ToDate)
    )
    {
        if (paymentParams.IsPaginated)
        {
            ApplyPaging(paymentParams.PageSize * (paymentParams.PageIndex - 1), paymentParams.PageSize);
        } 
        AddOrderByDescending(x => x.CreatedAtUtc);
    }
}
