using Grpc.Core;
using MediatR;
using OrdersService;
using Services.Orders.Application.BusinessLogics.Orders.Features.UpdateOrder;

namespace Services.Orders.Infrastructure.Services.Grpc.Server;
public class GrpcOrderService :GrpcOrder.GrpcOrderBase
{
    private readonly IMediator _mediator;
    public GrpcOrderService(IMediator mediator)
    {
        _mediator = mediator;
    }
    public override async Task<UpdateOrderStatusResponse> UpdateOrderStatus(UpdateOrderStatusRequest request, ServerCallContext context)
    {

        bool isSuccess = await _mediator.Send
            (
                new UpdateOrderCommand 
                { 
                    OrderCode = request.OrderCode, 
                    Status = request.Status
                }
            );
        var response = new UpdateOrderStatusResponse
        {
            IsSuccess = isSuccess
        };
        return response;
    }
}
