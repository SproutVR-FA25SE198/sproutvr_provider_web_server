using MediatR;

namespace Services.Orders.Application.BusinessLogics.Orders.Features.GetOrderById;
public class GetOrderByIdQuery(Guid id) : IRequest<OrderDetailsDto>
{
    public Guid Id { get; set; } = id;
}
