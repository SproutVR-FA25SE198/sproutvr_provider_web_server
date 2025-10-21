using Common.Application.Abstractions;
using Common.Application.Abstractions.Data;
using Common.Application.Helpers;
using Common.Domain.Entities;
using Common.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Services.Orders.Application.BusinessLogics.OrderItems.Mappings;
using Services.Orders.Application.BusinessLogics.OrderItems.Specifications;
using Services.Orders.Application.BusinessLogics.Orders.Features.CreateOrder;
using Services.Orders.Domain.Entities.OrderItems;

namespace Services.Orders.Application.BusinessLogics.Orders.Features.GetPurchasedMaps;
public class GetPurchasedMapsQueryHandler(IUnitOfWork unitOfWork, IUserContext userContext) : IRequestHandler<GetPurchasedMapsQuery, PaginatedResult<MapDto>>
{
    public async Task<PaginatedResult<MapDto>> Handle(GetPurchasedMapsQuery request, CancellationToken cancellationToken)
    {
        CurrentUser? userClaims = userContext.GetCurrentUser() ?? throw new UnauthorizedAccessException();
        request.SpecParams.OrganizationId = Guid.Parse(userClaims!.Id);
        
        // get purchased map list of current org
        IReadOnlyList<OrderItem> orderItemList = await unitOfWork.Repository<OrderItem>().ListAsync(new OrderItemSpecification(request.SpecParams));
        
        int totalCount = await unitOfWork.Repository<OrderItem>().CountAsync(new OrderItemSpecification(request.SpecParams));
        
        List<MapDto> result = orderItemList.Any() ? orderItemList.Select(x => x.ToMapDto()).ToList() : [];

        return new PaginatedResult<MapDto>(request.SpecParams.PageIndex, request.SpecParams.PageSize, totalCount, result);
    }
}
