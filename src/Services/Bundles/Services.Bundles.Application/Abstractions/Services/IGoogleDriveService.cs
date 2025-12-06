using System.IO.Compression;
using Google.Apis.Drive.v3;

namespace Services.Bundles.Application.Abstractions.Services;
public interface IGoogleDriveService
{
    DriveService GetDriveService();
    Task<string> UploadFileToDrive(DriveService service, ZipArchiveEntry entry, string folderId);
    Task<string> UploadFileToDriveFromBytes(DriveService service, byte[] fileContent, string fileName, string folderId);
    Task<string> CreateFolderInDrive(DriveService service, string folderName, string parentFolderId);
    Task<string> CopyFileToFolder(DriveService service, string sourceFileId, string targetFolderId, string? newFileName = null);
    Task<byte[]> DownloadFileFromDrive(DriveService service, string fileId);
    Task<string> GetShareableLinkForFolder(DriveService service, string folderId);
    Task<string> GetShareableLinkForFile(DriveService service, string fileId);
    
    #pragma warning disable CA1054 // URI-like parameters should not be strings
    string ConvertToDirectDownloadLink(string driveUrl);
    #pragma warning restore CA1054 // URI-like parameters should not be strings
}
