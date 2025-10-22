using AccountsService;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Services.Accounts.Application.Abstractions.Data.Repositories;

namespace Services.Accounts.Infrastructure.Services.Grpc;
public class GrpcAccountService : GrpcAccount.GrpcAccountBase
{
    private readonly ILogger<GrpcAccountService> _logger;
    private readonly ISystemAdminRepository _systemAdminRepository;

    public GrpcAccountService(ILogger<GrpcAccountService> logger, ISystemAdminRepository systemAdminRepository)
    {
        _logger = logger;
        _systemAdminRepository = systemAdminRepository;
    }

    public override async Task<GetSystemAdminWithMinPendingOrdersResponse> GetSystemAdminWithMinPendingOrders(
        GetSystemAdminWithMinPendingOrdersRequest request, 
        ServerCallContext context)
    {
        _logger.LogInformation("Getting system admin with minimum pending orders");

        try
        {
            Domain.Entities.SystemAdmins.SystemAdmin? systemAdmin = await _systemAdminRepository.GetSystemAdminWithMinPendingOrdersAsync();
            
            if (systemAdmin == null)
            {
                return new GetSystemAdminWithMinPendingOrdersResponse
                {
                    SystemAdminId = string.Empty,
                    FullName = string.Empty,
                    NumberOfPendingOrders = 0
                };
            }

            return new GetSystemAdminWithMinPendingOrdersResponse
            {
                SystemAdminId = systemAdmin.Id.ToString(),
                FullName = systemAdmin.FullName,
                NumberOfPendingOrders = systemAdmin.NumberOfPendingOrders
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting system admin with minimum pending orders");
            return new GetSystemAdminWithMinPendingOrdersResponse
            {
                SystemAdminId = string.Empty,
                FullName = string.Empty,
                NumberOfPendingOrders = 0
            };
        }
    }

    public override async Task<GetSystemAdminByIdResponse> GetSystemAdminById(
        GetSystemAdminByIdRequest request, 
        ServerCallContext context)
    {
        _logger.LogInformation("Getting system admin by id: {SystemAdminId}", request.SystemAdminId);

        try
        {
            var systemAdminId = Guid.Parse(request.SystemAdminId);
            Domain.Entities.SystemAdmins.SystemAdmin? systemAdmin = await _systemAdminRepository.GetByIdAsync(systemAdminId);
            
            if (systemAdmin == null)
            {
                return new GetSystemAdminByIdResponse
                {
                    SystemAdminId = string.Empty,
                    FullName = string.Empty,
                    NumberOfPendingOrders = 0
                };
            }

            return new GetSystemAdminByIdResponse
            {
                SystemAdminId = systemAdmin.Id.ToString(),
                FullName = systemAdmin.FullName,
                NumberOfPendingOrders = systemAdmin.NumberOfPendingOrders
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting system admin by id: {SystemAdminId}", request.SystemAdminId);
            return new GetSystemAdminByIdResponse
            {
                SystemAdminId = string.Empty,
                FullName = string.Empty,
                NumberOfPendingOrders = 0
            };
        }
    }
}
