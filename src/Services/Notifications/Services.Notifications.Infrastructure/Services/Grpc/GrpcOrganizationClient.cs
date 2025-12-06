using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Crypto;
using OrganizationAccountsService;
using Services.Notifications.Application.Abstractions.Grpc;

namespace Services.Notifications.Infrastructure.Services.Grpc;
public class GrpcOrganizationClient : IGrpcOrganizationClient
{
    private readonly ILogger<GrpcOrganizationClient> _logger;
    private readonly GrpcOrganization.GrpcOrganizationClient _client;

    public GrpcOrganizationClient(ILogger<GrpcOrganizationClient> logger,
        GrpcOrganization.GrpcOrganizationClient client)
    {
        _logger = logger;
        _client = client;
    }
    public async Task<GetOrganizationByIdResponse> GetOrganizationByIdAsync(string organizationId)
    {
        _logger.LogInformation("Calling GRPC Service to get organization info by id");


        try
        {
            // Make a request to Grpc Server
            GetOrganizationByIdResponse response = await _client.GetOrganizationByIdAsync(new GetOrganizationByIdRequest() { OranganizationId = organizationId });

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not call Grpc Server");
            return null;
        }
    }


}
