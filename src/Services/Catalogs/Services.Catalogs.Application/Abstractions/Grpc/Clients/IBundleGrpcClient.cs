namespace Services.Catalogs.Application.Abstractions.Grpc.Clients;

public interface IBundleGrpcClient
{
    Task<BundleUploadResult> UploadMetadataAsync(byte[] fileContent, string fileName, string mapCode);
}

public record BundleUploadResult(
    string StoragePath,
    bool Success,
    string ErrorMessage);

