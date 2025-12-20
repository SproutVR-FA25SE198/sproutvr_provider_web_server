using Common.Application.Abstractions.Data;
using Common.Application.Helpers;
using MediatR;
using Services.Payments.Application.Helpers;
using Services.Payments.Domain.Entities.Payments;

namespace Services.Payments.Application.BusinessLogics.GetPaymentsList;
public class GetPaymentsListQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetPaymentsListQuery, PaginatedResult<PaymentDto>>
{
    public async Task<PaginatedResult<PaymentDto>> Handle(GetPaymentsListQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<PaymentTransaction> paymentList = await unitOfWork.Repository<PaymentTransaction>().ListAsync(new PaymentSpecification(request.SpecParams));
        int totalCount = await unitOfWork.Repository<PaymentTransaction>().CountAsync(new PaymentSpecification(request.SpecParams));
        List<PaymentDto> result = paymentList.Any() ? paymentList.Select(x => x.ToDto()).ToList() : [];

        return new PaginatedResult<PaymentDto>(request.SpecParams.PageIndex, request.SpecParams.PageSize, totalCount, result);
    }
}
