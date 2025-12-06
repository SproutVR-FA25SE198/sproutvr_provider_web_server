using Common.Application.Abstractions;
using Common.Application.Abstractions.Data;
using Common.Application.Helpers;
using Common.Domain.Entities;
using MediatR;
using Services.Orders.Application.BusinessLogics.OrderItems.Specifications;
using Services.Orders.Application.BusinessLogics.Orders.Features.GetOrders;
using Services.Orders.Application.BusinessLogics.Orders.Mappings;
using Services.Orders.Application.BusinessLogics.Orders.Specifications;
using Services.Orders.Domain.Entities.OrderItems;
using Services.Orders.Domain.Entities.Orders;

namespace Services.Orders.Application.BusinessLogics.Orders.Features.GetOrderHistory;
public class GetOrderHistoryQueryHandler(IUnitOfWork unitOfWork, IUserContext userContext) : IRequestHandler<GetOrderHistoryQuery, PaginatedResult<OrderDto>>
{
    // get order history of current org
    public async Task<PaginatedResult<OrderDto>> Handle(GetOrderHistoryQuery request, CancellationToken cancellationToken)
    {
        CurrentUser? userClaims = userContext.GetCurrentUser() ?? throw new UnauthorizedAccessException();
        request.SpecParams.OrganizationId = Guid.Parse(userClaims!.Id);

        // get purchased map list of current org
        IReadOnlyList<Order> orderList = await unitOfWork.Repository<Order>().ListAsync(new OrderSpecification(request.SpecParams));

        int totalCount = await unitOfWork.Repository<Order>().CountAsync(new OrderSpecification(request.SpecParams));

        List<OrderDto> result = orderList.Any() ? orderList.Select(x => x.ToDto()).ToList() : [];

        return new PaginatedResult<OrderDto>(request.SpecParams.PageIndex, request.SpecParams.PageSize, totalCount, result);
    }
}
