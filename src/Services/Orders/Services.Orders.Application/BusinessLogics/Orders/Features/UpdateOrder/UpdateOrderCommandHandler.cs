using Common.Application.Abstractions.Data;
using MediatR;
using Services.Orders.Application.BusinessLogics.Orders.Specifications;
using Services.Orders.Domain.Entities.Orders;

namespace Services.Orders.Application.BusinessLogics.Orders.Features.UpdateOrder;
public class UpdateOrderCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateOrderCommand, bool>
{
    public async Task<bool> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            Order order = await unitOfWork.Repository<Order>().GetEntityWithSpec
                (
                    new OrderSpecification
                        (
                            request.OrderCode
                        )
                );
            if (order == null)
            {
                return false;
            }
            order.Status = Enum.Parse<OrderStatus>(request.Status);
            unitOfWork.Repository<Order>().Update(order);

            bool result = await unitOfWork.SaveChangesAsync(cancellationToken);
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while updating the order: {ex.Message}");
            return false;
        }
    }
}
