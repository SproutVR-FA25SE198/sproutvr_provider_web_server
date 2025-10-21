using Common.Application.Abstractions.Data;
using Common.Application.Contracts.Orders;
using MassTransit;
using MediatR;
using Services.Orders.Application.BusinessLogics.Orders.Mappings;
using Services.Orders.Application.BusinessLogics.Orders.Specifications;
using Services.Orders.Domain.Entities.Orders;

namespace Services.Orders.Application.BusinessLogics.Orders.Features.UpdateOrder;
public class UpdateOrderCommandHandler(IUnitOfWork unitOfWork, 
    IPublishEndpoint publishEndpoint) : IRequestHandler<UpdateOrderCommand, bool>
{
    public async Task<bool> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
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
        
        OrderStatus oldStatus = order.Status;
        OrderStatus newStatus = Enum.Parse<OrderStatus>(request.Status);
        
        order.Status = newStatus;
        unitOfWork.Repository<Order>().Update(order);

        // Only send notification when order moves to Bundle_Pending status
        if (newStatus == OrderStatus.Bundle_Pending && oldStatus != OrderStatus.Bundle_Pending)
        {
            OrderCreatedMessage orderCreatedMessage = OrderMappings.ToMessage(order);

            // send notification to system admins and invoice email to organization
            await publishEndpoint.Publish(orderCreatedMessage, cancellationToken);
        }

        bool result = await unitOfWork.SaveChangesAsync(cancellationToken);

        return result;

    }
}
