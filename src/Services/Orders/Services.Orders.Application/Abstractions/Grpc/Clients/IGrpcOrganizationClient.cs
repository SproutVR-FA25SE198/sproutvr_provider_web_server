namespace Services.Orders.Application.Abstractions.Grpc.Clients;

public interface IGrpcOrganizationClient
{
    Task<string> GetOrganizationBundleDriveIdAsync(Guid organizationId, CancellationToken cancellationToken = default);
}

