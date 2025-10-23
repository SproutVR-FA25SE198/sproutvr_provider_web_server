using AccountsService;
using Microsoft.Extensions.Logging;
using Services.Orders.Application.Abstractions.Grpc.Clients;
using Services.Orders.Application.BusinessLogics.Orders.Features.AssignSystemAdmin;

namespace Services.Orders.Infrastructure.Services.Grpc.Client;
public class GrpcAccountClient : IGrpcAccountClient
{
    private readonly ILogger<GrpcAccountClient> _logger;
    private readonly GrpcAccount.GrpcAccountClient _client;

    public GrpcAccountClient(ILogger<GrpcAccountClient> logger,
        GrpcAccount.GrpcAccountClient client)
    {
        _logger = logger;
        _client = client;
    }

    public async Task<SystemAdminDto?> GetSystemAdminWithMinPendingOrdersAsync()
    {
        _logger.LogInformation("Calling GRPC Service to get system admin with min pending orders");

        try
        {
            var request = new GetSystemAdminWithMinPendingOrdersRequest();
            GetSystemAdminWithMinPendingOrdersResponse response = await _client.GetSystemAdminWithMinPendingOrdersAsync(request);

            if (string.IsNullOrEmpty(response.SystemAdminId))
            {
                return null;
            }

            return new SystemAdminDto
            {
                SystemAdminId = Guid.Parse(response.SystemAdminId),
                FullName = response.FullName,
                NumberOfPendingOrders = response.NumberOfPendingOrders
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not call Grpc Server");
            return null;
        }
    }
}
