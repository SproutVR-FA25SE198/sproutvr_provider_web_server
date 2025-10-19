using OrganizationAccountsService;

namespace Services.Notifications.Application.Abstractions.Grpc;
public interface IGrpcOrganizationClient
{
    Task<GetOrganizationByIdResponse> GetOrganizationByIdAsync(string organizationId);
}
