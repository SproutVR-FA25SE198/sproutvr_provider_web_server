using System.IO.Compression;
using Google.Apis.Drive.v3;
using Services.Bundles.Application.Abstractions.Services;
using Services.Bundles.Infrastructure.Helpers;

namespace Services.Bundles.Infrastructure.Services;

public class GoogleDriveService : IGoogleDriveService
{
    private readonly OAuthHelper _oAuthHelper;

    public GoogleDriveService(OAuthHelper oAuthHelper)
    {
        _oAuthHelper = oAuthHelper;
    }
    public DriveService GetDriveService()
    {
        //return _oAuthHelper.GetInitAuthLocal(); //local once first
        return _oAuthHelper.GetAuthCloud();
    }

    public async Task<string> UploadFileToDrive(DriveService service, ZipArchiveEntry entry, string folderId)
    {
        var fileMetadata = new Google.Apis.Drive.v3.Data.File()
        {
            Name = entry.Name,
            Parents = new List<string> { folderId }
        };

        await using Stream entryStream = entry.Open();
        FilesResource.CreateMediaUpload request = service.Files.Create(fileMetadata, entryStream, "application/octet-stream");
        request.Fields = "id";
        request.SupportsAllDrives = true;
        Google.Apis.Upload.IUploadProgress result = await request.UploadAsync();

        if (result.Status != Google.Apis.Upload.UploadStatus.Completed)
        {
            throw new Exception($"Lỗi upload file {entry.Name}: {result.Exception.Message}");
        }

        Google.Apis.Drive.v3.Data.File file = request.ResponseBody;
        return file.Id;
    }

    public async Task<string> CreateFolderInDrive(DriveService service, string folderName, string parentFolderId)
    {
        var fileMetadata = new Google.Apis.Drive.v3.Data.File()
        {
            Name = folderName,
            MimeType = "application/vnd.google-apps.folder",
            Parents = new List<string> { parentFolderId }
        };

        FilesResource.CreateRequest request = service.Files.Create(fileMetadata);
        request.Fields = "id";
        request.SupportsAllDrives = true;
        Google.Apis.Drive.v3.Data.File folder = await request.ExecuteAsync();
        return folder.Id;
    }
}
