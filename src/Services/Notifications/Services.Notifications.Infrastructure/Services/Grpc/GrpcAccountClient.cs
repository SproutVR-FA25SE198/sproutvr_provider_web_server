using AccountsService;
using Microsoft.Extensions.Logging;
using Services.Notifications.Application.Abstractions.Grpc;
using Services.Notifications.Application.BusinessLogics.SystemAdmins;

namespace Services.Notifications.Infrastructure.Services.Grpc;
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

    public async Task<SystemAdminDto?> GetSystemAdminByIdAsync(Guid systemAdminId)
    {
        _logger.LogInformation("Calling GRPC Service to get system admin by id: {SystemAdminId}", systemAdminId);

        try
        {
            var request = new GetSystemAdminByIdRequest
            {
                SystemAdminId = systemAdminId.ToString()
            };
            
            GetSystemAdminByIdResponse response = await _client.GetSystemAdminByIdAsync(request);

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
