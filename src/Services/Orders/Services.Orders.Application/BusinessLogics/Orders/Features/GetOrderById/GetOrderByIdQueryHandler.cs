using Common.Application.Abstractions.Data;
using Common.Domain.Exceptions;
using MediatR;
using Services.Orders.Application.BusinessLogics.Orders.Mappings;
using Services.Orders.Application.BusinessLogics.Orders.Specifications;
using Services.Orders.Domain.Entities.Orders;

namespace Services.Orders.Application.BusinessLogics.Orders.Features.GetOrderById;
public class GetOrderByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetOrderByIdQuery, OrderDetailsDto>
{
    public async Task<OrderDetailsDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var spec = new OrderSpecification(request.Id);
        Order order = await unitOfWork.Repository<Order>().GetEntityWithSpec(spec);

        if (order == null)
        {
            throw new NotFoundException("Order not found!");
        }

        return order.ToDetailsDto();
    }
}
