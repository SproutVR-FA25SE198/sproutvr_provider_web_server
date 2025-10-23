using Services.Notifications.Application.BusinessLogics.SystemAdmins;

namespace Services.Notifications.Application.Abstractions.Grpc;
public interface IGrpcAccountClient
{
    Task<SystemAdminDto?> GetSystemAdminByIdAsync(Guid systemAdminId);
}
