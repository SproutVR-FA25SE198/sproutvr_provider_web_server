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
        return _oAuthHelper.GetDriveService();
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

    public async Task<string> UploadFileToDriveFromBytes(DriveService service, byte[] fileContent, string fileName, string folderId)
    {
        var fileMetadata = new Google.Apis.Drive.v3.Data.File()
        {
            Name = fileName,
            Parents = new List<string> { folderId }
        };

        // Upload from memory stream (file content as bytes)
        await using var memoryStream = new MemoryStream(fileContent);
        FilesResource.CreateMediaUpload request = service.Files.Create(fileMetadata, memoryStream, "application/zip");
        request.Fields = "id";
        request.SupportsAllDrives = true;
        Google.Apis.Upload.IUploadProgress result = await request.UploadAsync();

        if (result.Status != Google.Apis.Upload.UploadStatus.Completed)
        {
            throw new Exception($"Lỗi upload file {fileName}: {result.Exception.Message}");
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

    public async Task<string> CopyFileToFolder(DriveService service, string sourceFileId, string targetFolderId, string? newFileName = null)
    {
        var fileMetadata = new Google.Apis.Drive.v3.Data.File()
        {
            Parents = new List<string> { targetFolderId }
        };

        if (!string.IsNullOrEmpty(newFileName))
        {
            fileMetadata.Name = newFileName;
        }

        FilesResource.CopyRequest request = service.Files.Copy(fileMetadata, sourceFileId);
        request.Fields = "id, name";
        request.SupportsAllDrives = true;
        Google.Apis.Drive.v3.Data.File copiedFile = await request.ExecuteAsync();
        return copiedFile.Id;
    }

    public async Task<byte[]> DownloadFileFromDrive(DriveService service, string fileId)
    {
        FilesResource.GetRequest request = service.Files.Get(fileId);
        request.SupportsAllDrives = true;
        
        using var memoryStream = new MemoryStream();
        await request.DownloadAsync(memoryStream);
        return memoryStream.ToArray();
    }

    public async Task<string> GetShareableLinkForFolder(DriveService service, string folderId)
    {
        // Set folder to be accessible by anyone with the link
        var permission = new Google.Apis.Drive.v3.Data.Permission()
        {
            Type = "anyone",
            Role = "reader"
        };

        PermissionsResource.CreateRequest permissionRequest = service.Permissions.Create(permission, folderId);
        permissionRequest.SupportsAllDrives = true;
        await permissionRequest.ExecuteAsync();

        // Get the file metadata to retrieve the web view link
        FilesResource.GetRequest fileRequest = service.Files.Get(folderId);
        fileRequest.Fields = "webViewLink";
        fileRequest.SupportsAllDrives = true;
        Google.Apis.Drive.v3.Data.File file = await fileRequest.ExecuteAsync();

        return file.WebViewLink ?? string.Empty;
    }

    public async Task<string> GetShareableLinkForFile(DriveService service, string fileId)
    {
        // Set file to be accessible by anyone with the link
        var permission = new Google.Apis.Drive.v3.Data.Permission()
        {
            Type = "anyone",
            Role = "reader"
        };

        PermissionsResource.CreateRequest permissionRequest = service.Permissions.Create(permission, fileId);
        permissionRequest.SupportsAllDrives = true;
        await permissionRequest.ExecuteAsync();

        // Get the file metadata to retrieve the web view link
        FilesResource.GetRequest fileRequest = service.Files.Get(fileId);
        fileRequest.Fields = "webViewLink";
        fileRequest.SupportsAllDrives = true;
        Google.Apis.Drive.v3.Data.File file = await fileRequest.ExecuteAsync();

        return file.WebViewLink ?? string.Empty;
    }

    public string ConvertToDirectDownloadLink(string driveUrl)
    {
        if (string.IsNullOrWhiteSpace(driveUrl))
        {
            return string.Empty;
        }

        // Extract file/folder ID from Google Drive URL
        // Format: https://drive.google.com/file/d/{FILE_ID}/view or https://drive.google.com/drive/folders/{FOLDER_ID}
        System.Text.RegularExpressions.Match fileIdMatch = System.Text.RegularExpressions.Regex.Match(
            driveUrl, 
            @"(?:file/d/|folders/)([^/\?]+)");

        if (!fileIdMatch.Success)
        {
            return driveUrl;
        }

        string fileId = fileIdMatch.Groups[1].Value;
        
        // Generate direct download URL
        return $"https://drive.google.com/uc?export=download&id={fileId}";
    }
}
