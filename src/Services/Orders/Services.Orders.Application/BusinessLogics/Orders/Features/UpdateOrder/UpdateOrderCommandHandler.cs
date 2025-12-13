using Common.Application.Abstractions.Data;
using Common.Application.Contracts.Orders;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Services.Orders.Application.Abstractions.Grpc.Clients;
using Services.Orders.Application.Abstractions.Services;
using Services.Orders.Application.BusinessLogics.Orders.Features.AssignSystemAdmin;
using Services.Orders.Application.BusinessLogics.Orders.Mappings;
using Services.Orders.Application.BusinessLogics.Orders.Specifications;
using Services.Orders.Domain.Entities.Orders;

namespace Services.Orders.Application.BusinessLogics.Orders.Features.UpdateOrder;
public class UpdateOrderCommandHandler(
    IUnitOfWork unitOfWork, 
    IPublishEndpoint publishEndpoint, 
    IGrpcAccountClient grpcAccountClient,
    IGrpcOrganizationClient grpcOrganizationClient,
    IActivationKeyGeneratorService activationKeyGeneratorService,
    IConfiguration configuration,
    ILogger<UpdateOrderCommandHandler> logger) : IRequestHandler<UpdateOrderCommand, UpdateOrderResponseDto>
{
    public async Task<UpdateOrderResponseDto> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
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
            return new UpdateOrderResponseDto() { IsSuccess = false, OrderId = null};
        }
        
        OrderStatus oldStatus = order.Status;
        OrderStatus newStatus = Enum.Parse<OrderStatus>(request.Status);
        
        order.Status = newStatus;
        unitOfWork.Repository<Order>().Update(order);

        // if order is paid (status changed to Bundle_Pending), check auto-prepare flag
        if (newStatus == OrderStatus.Bundle_Pending && oldStatus != OrderStatus.Bundle_Pending)
        {
            // Read autoPrepareOrder flag from configuration
            bool autoPrepareOrder = configuration.GetValue<bool>("AutoPrepareOrder", true);

            logger.LogInformation(
                "Order #{OrderCode} status changed to Bundle_Pending. AutoPrepareOrder flag: {AutoPrepareOrder}",
                order.OrderCode,
                autoPrepareOrder);

            if (autoPrepareOrder)
            {
                // AUTOMATIC FLOW: Publish event to Bundles service for auto-preparation
                logger.LogInformation("Auto-prepare enabled. Publishing OrderCreated event for automatic preparation.");
                
                // Get organization bundle drive ID via gRPC
                string orgBundleDriveId = await grpcOrganizationClient.GetOrganizationBundleDriveIdAsync(
                    order.OrganizationId, 
                    cancellationToken);
                
                await PublishOrderCreatedEvent(order, orgBundleDriveId, cancellationToken);
            }
            else
            {
                // MANUAL FLOW: Assign to system admin for manual APK upload
                logger.LogInformation("Auto-prepare disabled. Assigning order to system admin for manual preparation.");
                
                // get system admin with min number of pending orders
                SystemAdminDto? systemAdmin = await grpcAccountClient.GetSystemAdminWithMinPendingOrdersAsync();

                if (systemAdmin != null)
                {
                    order.AssignedSystemAdminId = systemAdmin.SystemAdminId;
                    unitOfWork.Repository<Order>().Update(order);
                    await unitOfWork.SaveChangesAsync(cancellationToken);
                    
                    logger.LogInformation(
                        "Order #{OrderCode} assigned to System Admin {AdminId}",
                        order.OrderCode,
                        systemAdmin.SystemAdminId);
                }
                
                // Get organization bundle drive ID via gRPC
                string orgBundleDriveId = await grpcOrganizationClient.GetOrganizationBundleDriveIdAsync(
                    order.OrganizationId, 
                    cancellationToken);
                
                await PublishOrderCreatedEvent(order, orgBundleDriveId, cancellationToken);
            }
        }
        else if (newStatus == OrderStatus.Finished && oldStatus != OrderStatus.Finished)
        {
            // Generate the key
            string activationKey = activationKeyGeneratorService.Generate();

            // Assign the key to the order entity
            order.ActivationKey = activationKey;
        }

        bool result = await unitOfWork.SaveChangesAsync(cancellationToken);

        return new UpdateOrderResponseDto() { IsSuccess = result, OrderId = order.Id};

    }

    private async Task PublishOrderCreatedEvent(Order order, string orgBundleDriveId, CancellationToken cancellationToken)
    {
        // send order created message to message broker
        OrderCreatedMessage orderCreatedMessage = OrderMappings.ToMessage(order, orgBundleDriveId);

        // publish message
        await publishEndpoint.Publish(orderCreatedMessage, cancellationToken);
        // consumers:
        // account service - to update number of pending orders for assigned system admin
        // notification service - send notification to the assigned system admin and invoice email to organization
        // bundles service (if auto-prepare enabled) - to automatically prepare metadata
    }
}
