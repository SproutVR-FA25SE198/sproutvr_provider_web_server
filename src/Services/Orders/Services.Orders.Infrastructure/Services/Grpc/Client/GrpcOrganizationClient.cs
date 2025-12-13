using Microsoft.Extensions.Logging;
using OrganizationAccountsService;
using Services.Orders.Application.Abstractions.Grpc.Clients;

namespace Services.Orders.Infrastructure.Services.Grpc.Client;

public class GrpcOrganizationClient : IGrpcOrganizationClient
{
    private readonly GrpcOrganization.GrpcOrganizationClient _grpcClient;
    private readonly ILogger<GrpcOrganizationClient> _logger;

    public GrpcOrganizationClient(
        GrpcOrganization.GrpcOrganizationClient grpcClient,
        ILogger<GrpcOrganizationClient> logger)
    {
        _grpcClient = grpcClient;
        _logger = logger;
    }

    public async Task<string> GetOrganizationBundleDriveIdAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Fetching bundle drive ID for organization {OrgId}", organizationId);

            var request = new GetOrganizationByIdRequest
            {
                OranganizationId = organizationId.ToString()
            };

            GetOrganizationByIdResponse response = await _grpcClient.GetOrganizationByIdAsync(
                request, 
                cancellationToken: cancellationToken);

            if (string.IsNullOrEmpty(response.BundleGoogleDriveId))
            {
                _logger.LogWarning("Organization {OrgId} has no bundle drive ID set", organizationId);
                return string.Empty;
            }

            _logger.LogInformation(
                "Successfully fetched bundle drive ID for organization {OrgId}: {DriveId}", 
                organizationId, 
                response.BundleGoogleDriveId);

            return response.BundleGoogleDriveId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch bundle drive ID for organization {OrgId}", organizationId);
            return string.Empty;
        }
    }
}

