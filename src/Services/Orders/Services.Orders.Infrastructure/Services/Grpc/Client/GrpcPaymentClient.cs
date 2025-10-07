using Microsoft.Extensions.Logging;
using PaymentsService;
using Services.Orders.Application.Abstractions.Grpc.Clients;
using Services.Orders.Application.BusinessLogics.Orders.Features.CreateOrder;

namespace Services.Orders.Infrastructure.Services.Grpc.Client;
public class GrpcPaymentClient : IGrpcPaymentClient
{
    private readonly ILogger<GrpcPaymentClient> _logger;
    private readonly GrpcPayment.GrpcPaymentClient _client;
    public GrpcPaymentClient(ILogger<GrpcPaymentClient> logger, 
        GrpcPayment.GrpcPaymentClient client)
    {
        _logger = logger;
        _client = client;
    }
    public async Task<OrderResponseDto> CreatePaymentAsync(CreatePaymentRequest request)
    {
        _logger.LogInformation("Calling GRPC Service to create payment link");

        try
        {
            // Make a request to Grpc Server
            CreatePaymentResponse response = await _client.CreatePaymentAsync(request);
            return new OrderResponseDto { PaymentUrl = response.PaymentUrl };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not call Grpc Server");
            return new OrderResponseDto { PaymentUrl = string.Empty };
        }
    }
}
