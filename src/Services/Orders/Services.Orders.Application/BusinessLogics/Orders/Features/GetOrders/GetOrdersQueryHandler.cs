using Common.Application.Abstractions.Data;
using Common.Application.Helpers;
using MediatR;
using Services.Orders.Application.BusinessLogics.Orders.Mappings;
using Services.Orders.Application.BusinessLogics.Orders.Specifications;
using Services.Orders.Domain.Entities.Orders;

namespace Services.Orders.Application.BusinessLogics.Orders.Features.GetOrders;
public class GetOrdersQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetOrdersQuery, PaginatedResult<OrderDto>>
{
    public async Task<PaginatedResult<OrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<Order> orderList = await unitOfWork.Repository<Order>().ListAsync(new OrderSpecification(request.SpecParams));
        int totalCount = await unitOfWork.Repository<Order>().CountAsync(new OrderSpecification(request.SpecParams));
        List<OrderDto> result = orderList.Any() ? orderList.Select(x => x.ToDto()).ToList() : [];

        return new PaginatedResult<OrderDto>(request.SpecParams.PageIndex, request.SpecParams.PageSize, totalCount, result);
    }
}
