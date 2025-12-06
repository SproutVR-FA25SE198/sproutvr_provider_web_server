using MediatR;

namespace Services.Bundles.Application.BusinessLogics.UploadMetadata;

public record UploadMetadataCommand(
    byte[] FileContent,
    string FileName,
    string MapCode) : IRequest<UploadMetadataResult>;

public record UploadMetadataResult(
    string StoragePath,
    bool Success,
    string ErrorMessage);

