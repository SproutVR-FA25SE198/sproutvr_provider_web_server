using Common.Application.Helpers;
using MediatR;
using Services.Orders.Application.BusinessLogics.Orders.Features.GetOrders;
using Services.Orders.Application.BusinessLogics.Orders.Specifications;

namespace Services.Orders.Application.BusinessLogics.Orders.Features.GetOrderHistory;
public class GetOrderHistoryQuery : IRequest<PaginatedResult<OrderDto>>
{
    public OrderHistoryParams SpecParams { get; set; }
    public GetOrderHistoryQuery(OrderHistoryParams specParams)
    {
        SpecParams = specParams;
    }
}
