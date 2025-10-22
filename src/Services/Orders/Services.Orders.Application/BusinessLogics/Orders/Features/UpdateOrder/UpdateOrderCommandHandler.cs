using Common.Application.Abstractions.Data;
using Common.Application.Contracts.Orders;
using MassTransit;
using MediatR;
using Services.Orders.Application.Abstractions.Grpc.Clients;
using Services.Orders.Application.BusinessLogics.Orders.Features.AssignSystemAdmin;
using Services.Orders.Application.BusinessLogics.Orders.Mappings;
using Services.Orders.Application.BusinessLogics.Orders.Specifications;
using Services.Orders.Domain.Entities.Orders;

namespace Services.Orders.Application.BusinessLogics.Orders.Features.UpdateOrder;
public class UpdateOrderCommandHandler(IUnitOfWork unitOfWork, 
    IPublishEndpoint publishEndpoint, IGrpcAccountClient grpcAccountClient) : IRequestHandler<UpdateOrderCommand, bool>
{
    public async Task<bool> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        // get order by order code
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

        // if order is paid (status changed to Bundle_Pending), publish OrderCreated event
        if (newStatus == OrderStatus.Bundle_Pending && oldStatus != OrderStatus.Bundle_Pending)
        {
            // get system admin with min number of pending orders
            SystemAdminDto? systemAdmin = await grpcAccountClient.GetSystemAdminWithMinPendingOrdersAsync();

            if (systemAdmin != null)
            {
                order.AssignedSystemAdminId = systemAdmin.SystemAdminId;
                unitOfWork.Repository<Order>().Update(order);
                await unitOfWork.SaveChangesAsync(cancellationToken);
            }
            await PublishOrderCreatedEvent(order, cancellationToken);
        }

        bool result = await unitOfWork.SaveChangesAsync(cancellationToken);

        return result;

    }

    private async Task PublishOrderCreatedEvent(Order order, CancellationToken cancellationToken)
    {

        // send order created message to message broker
        OrderCreatedMessage orderCreatedMessage = OrderMappings.ToMessage(order);

        // publish message
        await publishEndpoint.Publish(orderCreatedMessage, cancellationToken);
        // consumers:
        // account service - to update number of pending orders for assigned system admin
        // notification service - send notification to the assigned system admin and invoice email to organization
    }


}
