using MediatR;

namespace Services.Orders.Application.BusinessLogics.Orders.Features.CreateOrder;
public class CreateOrderCommand(CreateOrderDto createOrderDto) : IRequest<OrderResponseDto>
{
    public CreateOrderDto CreateOrderDto { get; } = createOrderDto;
}
