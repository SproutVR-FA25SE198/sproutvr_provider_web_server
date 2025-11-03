using Grpc.Core;
using MediatR;
using Microsoft.Extensions.Logging;
using Services.Bundles.Application.BusinessLogics.UploadMetadata;
using Services.Bundles.Application.Protos;

namespace Services.Bundles.Infrastructure.Services.Grpc.Server;

public class BundleGrpcService : BundleService.BundleServiceBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<BundleGrpcService> _logger;

    public BundleGrpcService(IMediator mediator, ILogger<BundleGrpcService> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public override async Task<UploadMetadataResponse> UploadMetadata(
        UploadMetadataRequest request,
        ServerCallContext context)
    {
        _logger.LogInformation(
            "Received UploadMetadata gRPC request for map: {MapCode}, File: {FileName}, Size: {FileSize} bytes",
            request.MapCode,
            request.FileName,
            request.FileContent.Length);

        try
        {
            // Convert protobuf ByteString to byte array
            var command = new UploadMetadataCommand(
                request.FileContent.ToByteArray(),
                request.FileName,
                request.MapCode);

            UploadMetadataResult result = await _mediator.Send(command, context.CancellationToken);

            return new UploadMetadataResponse
            {
                StoragePath = result.StoragePath,
                Success = result.Success,
                ErrorMessage = result.ErrorMessage
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing UploadMetadata gRPC request for map: {MapCode}", request.MapCode);
            
            return new UploadMetadataResponse
            {
                StoragePath = string.Empty,
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
}

