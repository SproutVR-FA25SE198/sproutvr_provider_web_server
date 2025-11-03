using System.IO.Compression;
using Google.Apis.Drive.v3;

namespace Services.Bundles.Application.Abstractions.Services;
public interface IGoogleDriveService
{
    DriveService GetDriveService();
    Task<string> UploadFileToDrive(DriveService service, ZipArchiveEntry entry, string folderId);
    Task<string> UploadFileToDriveFromBytes(DriveService service, byte[] fileContent, string fileName, string folderId);
    Task<string> CreateFolderInDrive(DriveService service, string folderName, string parentFolderId);


}
