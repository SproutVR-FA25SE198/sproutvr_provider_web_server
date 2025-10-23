using Services.Orders.Application.BusinessLogics.Orders.Features.AssignSystemAdmin;

namespace Services.Orders.Application.Abstractions.Grpc.Clients;
public interface IGrpcAccountClient
{
    Task<SystemAdminDto?> GetSystemAdminWithMinPendingOrdersAsync();
}
