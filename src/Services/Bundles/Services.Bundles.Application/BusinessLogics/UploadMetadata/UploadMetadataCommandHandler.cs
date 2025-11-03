using Google.Apis.Drive.v3;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Services.Bundles.Application.Abstractions.Services;

namespace Services.Bundles.Application.BusinessLogics.UploadMetadata;

public class UploadMetadataCommandHandler : IRequestHandler<UploadMetadataCommand, UploadMetadataResult>
{
    private readonly IGoogleDriveService _googleDriveService;
    private readonly ILogger<UploadMetadataCommandHandler> _logger;
    private readonly string _metadataFolderId;

    public UploadMetadataCommandHandler(
        IGoogleDriveService googleDriveService,
        ILogger<UploadMetadataCommandHandler> logger,
        IConfiguration configuration)
    {
        _googleDriveService = googleDriveService;
        _logger = logger;
        #pragma warning disable CS8601 // Possible null reference assignment.
        _metadataFolderId = configuration["GoogleDrive:MetadataFolderId"];
        #pragma warning restore CS8601 // Possible null reference assignment.
    }

    public async Task<UploadMetadataResult> Handle(UploadMetadataCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Starting metadata upload for map: {MapCode}, File: {FileName}, Size: {FileSize} bytes", 
                request.MapCode, 
                request.FileName,
                request.FileContent.Length);

            // Validate file content
            if (request.FileContent == null || request.FileContent.Length == 0)
            {
                string error = "File content is empty";
                return new UploadMetadataResult(
                    string.Empty,
                    false,
                    error);
            }

            // Get Drive service
            DriveService driveService = _googleDriveService.GetDriveService();

            // Upload file content to Metadata folder on Google Drive
            // File is uploaded as ZIP format (content type: application/zip)
            string fileId = await _googleDriveService.UploadFileToDriveFromBytes(
                driveService,
                request.FileContent,
                request.FileName,
                _metadataFolderId);

            // Construct storage path (Drive file ID or web view link)
            string storagePath = $"https://drive.google.com/file/d/{fileId}/view";

            _logger.LogInformation(
                "Successfully uploaded metadata for map {MapCode}. File ID: {FileId}, Size: {FileSize} bytes",
                request.MapCode,
                fileId,
                request.FileContent.Length);

            return new UploadMetadataResult(
                storagePath,
                true,
                string.Empty);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading metadata for map: {MapCode}", request.MapCode);
            return new UploadMetadataResult(
                string.Empty,
                false,
                ex.Message);
        }
    }
}

