using Google.Protobuf;
using Microsoft.Extensions.Logging;
using Services.Catalogs.Application.Abstractions.Grpc.Clients;
using Services.Catalogs.Application.Protos.Bundles;

namespace Services.Catalogs.Infrastructure.Services.Grpc.Client;

public class BundleGrpcClient : IBundleGrpcClient
{
    private readonly ILogger<BundleGrpcClient> _logger;
    private readonly BundleService.BundleServiceClient _client;

    public BundleGrpcClient(
        ILogger<BundleGrpcClient> logger,
        BundleService.BundleServiceClient client)
    {
        _logger = logger;
        _client = client;
    }

    public async Task<BundleUploadResult> UploadMetadataAsync(byte[] fileContent, string fileName, string mapCode)
    {
        try
        {
            _logger.LogInformation(
                "Calling Bundle service gRPC to upload metadata for map: {MapCode}, File: {FileName}, Size: {FileSize} bytes",
                mapCode,
                fileName,
                fileContent.Length);

            var request = new UploadMetadataRequest
            {
                FileContent = ByteString.CopyFrom(fileContent),
                FileName = fileName,
                MapCode = mapCode
            };

            UploadMetadataResponse response = await _client.UploadMetadataAsync(request);

            _logger.LogInformation(
                "Bundle service gRPC call completed. Success: {Success}, StoragePath: {StoragePath}",
                response.Success,
                response.StoragePath);

            return new BundleUploadResult(
                response.StoragePath,
                response.Success,
                response.ErrorMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Bundle service gRPC for map: {MapCode}", mapCode);
            return new BundleUploadResult(
                string.Empty,
                false,
                $"gRPC call failed: {ex.Message}");
        }
    }
}

