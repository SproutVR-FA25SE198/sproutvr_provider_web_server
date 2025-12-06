using Common.Application.Abstractions.Data;
using Grpc.Core;
using MediatR;
using OrdersService;
using Services.Orders.Application.BusinessLogics.OrderItems.Specifications;
using Services.Orders.Application.BusinessLogics.Orders.Features.UpdateOrder;
using Services.Orders.Domain.Entities.OrderItems;

namespace Services.Orders.Infrastructure.Services.Grpc.Server;
public class GrpcOrderService :GrpcOrder.GrpcOrderBase
{
    private readonly IMediator _mediator;
    private readonly IUnitOfWork _unitOfWork;
    
    public GrpcOrderService(IMediator mediator, IUnitOfWork unitOfWork)
    {
        _mediator = mediator;
        _unitOfWork = unitOfWork;
    }
    
    public override async Task<UpdateOrderStatusResponse> UpdateOrderStatus(UpdateOrderStatusRequest request, ServerCallContext context)
    {
        UpdateOrderResponseDto result = await _mediator.Send
            (
                new UpdateOrderCommand 
                { 
                    OrderCode = request.OrderCode, 
                    Status = request.Status
                }
            );
        var response = new UpdateOrderStatusResponse
        {
            IsSuccess = result.IsSuccess,
            OrderId = result.OrderId.ToString(),
        };
        return response;
    }
    
    public override async Task<GetOrderItemsResponse> GetOrderItems(GetOrderItemsRequest request, ServerCallContext context)
    {
        Console.WriteLine($"[Orders gRPC] GetOrderItems called with OrderId: {request.OrderId}");
        
        try
        {
            if (!Guid.TryParse(request.OrderId, out Guid orderId))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid order ID format"));
            }

            Console.WriteLine($"[Orders gRPC] Querying database for OrderId: {orderId}");
            
            IReadOnlyList<OrderItem> orderItems = await _unitOfWork.Repository<OrderItem>()
                .ListAsync(new OrderItemSpecification(orderId));
            
            Console.WriteLine($"[Orders gRPC] Found {orderItems.Count} order items");

            var response = new GetOrderItemsResponse();
            
            foreach (OrderItem item in orderItems)
            {
                
                response.OrderItems.Add(new OrderItemModel
                {
                    Id = item.Id.ToString(),
                    OrderId = item.OrderId.ToString(),
                    MapId = item.MapId.ToString(),
                    MapCode = item.MapCode ?? string.Empty,
                    MapName = item.MapName ?? string.Empty,
                    SubjectName = item.SubjectName ?? string.Empty,
                    Price = (double)item.Price,
                    ImageUrl = item.ImageUrl ?? string.Empty,
                    DownloadUrl = item.DownloadUrl ?? string.Empty
                });
            }

            return response;
        }
        catch (RpcException ex)
        {
            Console.WriteLine($"[Orders gRPC] RpcException: {ex.Status}");
            throw;
        }
        catch (Exception ex)
        {
            if (ex.InnerException != null)
            {
                Console.WriteLine($"[Orders gRPC] InnerException: {ex.InnerException.GetType().FullName}");
                Console.WriteLine($"[Orders gRPC] InnerMessage: {ex.InnerException.Message}");
            }
            
            throw new RpcException(new Status(StatusCode.Internal, $"Error: {ex.Message}"));
        }
    }
    
    public override async Task<UpdateOrderItemsDownloadUrlResponse> UpdateOrderItemsDownloadUrl(
        UpdateOrderItemsDownloadUrlRequest request, 
        ServerCallContext context)
    {
        if (!Guid.TryParse(request.OrderId, out Guid orderId))
        {
            return new UpdateOrderItemsDownloadUrlResponse
            {
                IsSuccess = false,
                Message = "Invalid order ID format"
            };
        }

        try
        {
            IReadOnlyList<OrderItem> orderItems = await _unitOfWork.Repository<OrderItem>()
                .ListAsync(new OrderItemSpecification(orderId));

            if (orderItems.Count == 0)
            {
                return new UpdateOrderItemsDownloadUrlResponse
                {
                    IsSuccess = false,
                    Message = "No order items found for this order"
                };
            }

            // Update download URL for all order items
            foreach (OrderItem item in orderItems)
            {
                item.DownloadUrl = request.NewDownloadUrl;
                _unitOfWork.Repository<OrderItem>().Update(item);
            }

            await _unitOfWork.SaveChangesAsync();

            return new UpdateOrderItemsDownloadUrlResponse
            {
                IsSuccess = true,
                Message = $"Updated {orderItems.Count} order items successfully"
            };
        }
        catch (Exception ex)
        {
            return new UpdateOrderItemsDownloadUrlResponse
            {
                IsSuccess = false,
                Message = $"Error updating order items: {ex.Message}"
            };
        }
    }
    
    public override async Task<UpdateOrderItemDownloadUrlResponse> UpdateOrderItemDownloadUrl(
        UpdateOrderItemDownloadUrlRequest request, 
        ServerCallContext context)
    {
        try
        {
            if (!Guid.TryParse(request.OrderItemId, out Guid orderItemId))
            {
                return new UpdateOrderItemDownloadUrlResponse
                {
                    IsSuccess = false,
                    Message = "Invalid order item ID format"
                };
            }

            OrderItem? orderItem = await _unitOfWork.Repository<OrderItem>().GetByIdAsync(orderItemId);
            
            if (orderItem == null)
            {
                return new UpdateOrderItemDownloadUrlResponse
                {
                    IsSuccess = false,
                    Message = $"Order item not found with ID: {orderItemId}"
                };
            }

            // Update download URL for this specific order item
            orderItem.DownloadUrl = request.NewDownloadUrl;
            _unitOfWork.Repository<OrderItem>().Update(orderItem);
            
            bool saved = await _unitOfWork.SaveChangesAsync();
            
            if (!saved)
            {
                return new UpdateOrderItemDownloadUrlResponse
                {
                    IsSuccess = false,
                    Message = "Failed to save changes to database"
                };
            }

            return new UpdateOrderItemDownloadUrlResponse
            {
                IsSuccess = true,
                Message = $"Updated order item {orderItem.MapCode} successfully"
            };
        }
        catch (Exception ex)
        {
            // Log the full exception for debugging
            Console.WriteLine($"Error in UpdateOrderItemDownloadUrl: {ex}");
            return new UpdateOrderItemDownloadUrlResponse
            {
                IsSuccess = false,
                Message = $"Error updating order item: {ex.Message} | StackTrace: {ex.StackTrace}"
            };
        }
    }
}
