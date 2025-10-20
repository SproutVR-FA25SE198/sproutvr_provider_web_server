using Common.Application.Helpers;
using MediatR;
using Services.Orders.Application.BusinessLogics.OrderItems.Specifications;
using Services.Orders.Application.BusinessLogics.Orders.Features.CreateOrder;

namespace Services.Orders.Application.BusinessLogics.Orders.Features.GetPurchasedMaps;
public class GetPurchasedMapsQuery(OrderItemPurchasedMapParams specParams) : IRequest<PaginatedResult<MapDto>>
{
public OrderItemPurchasedMapParams SpecParams { get; set; } = specParams;
}
