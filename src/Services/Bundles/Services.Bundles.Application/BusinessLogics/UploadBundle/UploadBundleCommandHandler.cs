using System.IO.Compression;
using Common.Application.Contracts.Bundles;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Http;
using Services.Bundles.Application.Abstractions.Services;

namespace Services.Bundles.Application.BusinessLogics.UploadBundle;
public class UploadBundleCommandHandler(IPublishEndpoint publishEndpoint, IGoogleDriveService googleDriveService) : IRequestHandler<UploadBundleCommand>
{
    public async Task Handle(UploadBundleCommand request, CancellationToken cancellationToken)
    {
        IFormFile bundleFile = request.BundleFile;
        if (bundleFile == null || bundleFile.Length == 0)
        {
            throw new DirectoryNotFoundException("Bundle not found!");
        }

        // check bundle file is in .zip format
        if (!Path.GetExtension(bundleFile.FileName).Equals(".zip", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidDataException("Bundle must be in .zip format");
        }

        // Use MemoryStream to avoid temporary files on disk
        using var memoryStream = new MemoryStream();
        await bundleFile.CopyToAsync(memoryStream, cancellationToken);
        memoryStream.Position = 0;

        // --- 1. Extract file ---
        using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Read);

        ZipArchiveEntry? apkEntry = archive.Entries.FirstOrDefault(e => e.Name.EndsWith(".apk", StringComparison.OrdinalIgnoreCase));
        ZipArchiveEntry? jsonEntry = archive.Entries.FirstOrDefault(e => e.Name.EndsWith(".json", StringComparison.OrdinalIgnoreCase));

        if (apkEntry == null || jsonEntry == null)
        {
            throw new InvalidDataException("File bundle is invalid. Must contain an .apk file and a json file");
        }

        // --- 2. Upload file to Google Drive ---
        Google.Apis.Drive.v3.DriveService driveService = googleDriveService.GetDriveService();

        // Create a subfolder for this bundle, named with current timestamp
        string bundleSubfolderName = $"bundle_{DateTime.UtcNow:yyyyMMdd_HHmmss}";
        string subfolderId = await googleDriveService.CreateFolderInDrive(driveService, bundleSubfolderName, request.OrderDto.BundleGoogleDriveId);

        // Upload file APK
        await googleDriveService.UploadFileToDrive(driveService, apkEntry, subfolderId);

        // Upload file JSON
        await googleDriveService.UploadFileToDrive(driveService, jsonEntry, subfolderId);
    }

    // when bundle is uploaded, Bundle Service will call Order Service to trigger this function to update order status to Finished
    private async Task PublishBundleUploadedEvent(CancellationToken cancellationToken)
    {
        var orderFinishedMessage = new BundleUploadedMessage
        {
            // set properties accordingly
        };

        // publish message
        await publishEndpoint.Publish(orderFinishedMessage, cancellationToken);
        // consumers:
        // orders service - to update order status to Finished,
        // notification service - to notify,
        // accounts service - to update number of pending orders for assigned system admin
    }


}
