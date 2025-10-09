using Microsoft.Extensions.Logging;
using OrdersService;
using Services.Payments.Application.Abstractions.Grpc.Client;

namespace Services.Payments.Infrastructure.Services.Grpc.Client;
public class GrpcOrderClient : IGrpcOrderClient
{

    private readonly ILogger<GrpcOrderClient> _logger;
    private readonly GrpcOrder.GrpcOrderClient _client;
    public GrpcOrderClient(ILogger<GrpcOrderClient> logger,
        GrpcOrder.GrpcOrderClient client)
    {
        _logger = logger;
        _client = client;
    }
    public async Task<bool> UpdateOrderStatusAsync(UpdateOrderStatusRequest request)
    {
        _logger.LogInformation("Calling GRPC Service to update order status");

        try
        {
            // Make a request to Grpc Server
            UpdateOrderStatusResponse response = await _client.UpdateOrderStatusAsync(request);
            return response.IsSuccess;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not call Grpc Server");
            return false;
        }
    }
}
