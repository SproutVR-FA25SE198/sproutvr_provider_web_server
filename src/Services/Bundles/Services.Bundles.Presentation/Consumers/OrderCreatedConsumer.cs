using Common.Application.Contracts.Orders;
using MassTransit;
using MediatR;
using OrdersService;
using Services.Bundles.Application.BusinessLogics.CopyMetadataToOrder;

namespace Services.Bundles.Presentation.Consumers;

#pragma warning disable CA1515 // Consider making public types internal
public class OrderCreatedConsumer : IConsumer<OrderCreatedMessage>
#pragma warning restore CA1515 // Consider making public types internal
{
    private readonly IMediator _mediator;
    private readonly GrpcOrder.GrpcOrderClient _grpcOrderClient;
    private readonly ILogger<OrderCreatedConsumer> _logger;

    public OrderCreatedConsumer(
        IMediator mediator,
        GrpcOrder.GrpcOrderClient grpcOrderClient,
        ILogger<OrderCreatedConsumer> logger)
    {
        _mediator = mediator;
        _grpcOrderClient = grpcOrderClient;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderCreatedMessage> context)
    {
        try
        {
            OrderCreatedMessage message = context.Message;

            _logger.LogInformation(
                "Received OrderCreated message for Order #{OrderCode} (Org: {OrgId})",
                message.OrderCode,
                message.OrganizationId);

            // Check if bundle folder ID exists
            if (string.IsNullOrEmpty(message.BundleGoogleDriveId))
            {
                _logger.LogWarning("Order #{OrderCode} has no bundle Google Drive folder ID", message.OrderCode);
                return;
            }

            _logger.LogInformation(
                "Starting automatic metadata preparation for Order #{OrderCode}",
                message.OrderCode);

            // Use the CopyMetadataToOrder command to handle the metadata copying
            var copyMetadataCommand = new CopyMetadataToOrderCommand
            {
                OrderId = message.OrderId,
                BundleGoogleDriveFolderId = message.BundleGoogleDriveId
            };

            bool copySuccess = await _mediator.Send(copyMetadataCommand);

            if (!copySuccess)
            {
                _logger.LogError("Failed to copy metadata for Order #{OrderCode}", message.OrderCode);
                throw new InvalidOperationException($"Failed to copy metadata for Order #{message.OrderCode}");
            }

            _logger.LogInformation("Successfully prepared metadata for Order #{OrderCode}", message.OrderCode);

            // Update order status to Finished
            var updateOrderStatusRequest = new UpdateOrderStatusRequest
            {
                OrderCode = message.OrderCode ?? 0,
                Status = "Finished" // OrderStatus.Finished
            };

            UpdateOrderStatusResponse updateResponse = await _grpcOrderClient.UpdateOrderStatusAsync(updateOrderStatusRequest);

            if (!updateResponse.IsSuccess)
            {
                _logger.LogError(
                    "Failed to update order status to Finished for Order #{OrderCode}",
                    message.OrderCode);
                throw new InvalidOperationException($"Failed to update order status for Order #{message.OrderCode}");
            }

            _logger.LogInformation(
                "Automatic preparation completed for Order #{OrderCode}. Order status updated to Finished.",
                message.OrderCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to auto-prepare order #{OrderCode}",
                context.Message.OrderCode);
        }
    }
}

