using Common.Application.Helpers;
using MediatR;
using Services.Orders.Application.BusinessLogics.Orders.Specifications;

namespace Services.Orders.Application.BusinessLogics.Orders.Features.GetOrders;
public class GetOrdersQuery(OrderParams orderParams) : IRequest<PaginatedResult<OrderDto>>
{
    public OrderParams SpecParams { get; set; } = orderParams;
}
