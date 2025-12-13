using System.IO.Compression;
using Common.Application.Abstractions.Data;
using Common.Application.Contracts.Bundles;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Http;
using Services.Bundles.Application.Abstractions.Services;
using Services.Bundles.Application.BusinessLogics.CopyMetadataToOrder;

namespace Services.Bundles.Application.BusinessLogics.UploadBundle;
public class UploadBundleCommandHandler(
    IPublishEndpoint publishEndpoint, 
    IGoogleDriveService googleDriveService,
    IMediator mediator,
    IUnitOfWork unitOfWork) : IRequestHandler<UploadBundleCommand>
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

        // Check that zip contains exactly 1 APK file
        var apkEntries = archive.Entries.Where(e => e.Name.EndsWith(".apk", StringComparison.OrdinalIgnoreCase)).ToList();
        
        if (apkEntries.Count != 1)
        {
            throw new InvalidDataException("File bundle is invalid. Must contain only one .apk file");
        }
       

        ZipArchiveEntry apkEntry = apkEntries[0];

        // --- 2. Upload bundle APK to Google Drive ---
        Google.Apis.Drive.v3.DriveService driveService = googleDriveService.GetDriveService();

        // Create a subfolder for this bundle, named with current timestamp
        string bundleSubfolderName = $"bundle_{DateTime.UtcNow:yyyyMMdd_HHmmss}";
        string bundleFolderId = await googleDriveService.CreateFolderInDrive(driveService, bundleSubfolderName, request.OrderDto.BundleGoogleDriveId);

        // Upload the new bundle APK
        await googleDriveService.UploadFileToDrive(driveService, apkEntry, bundleFolderId);

        // --- 3. Copy metadata files to order items (using separate command) ---
        var copyMetadataCommand = new CopyMetadataToOrderCommand
        {
            OrderId = request.OrderDto.OrderId,
            BundleGoogleDriveFolderId = bundleFolderId
        };

        bool copySuccess = await mediator.Send(copyMetadataCommand, cancellationToken);

        if (!copySuccess)
        {
            throw new InvalidOperationException("Failed to copy metadata files to order items");
        }

        // --- 4. Publish BundleUploaded event ---
        await PublishBundleUploadedEvent(request, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

    }

    // when bundle is uploaded, Bundle Service will call Order Service to trigger this function to update order status to Finished
    private async Task PublishBundleUploadedEvent(UploadBundleCommand request, CancellationToken cancellationToken)
    {
        var bundleUploadedMessage = new BundleUploadedMessage
        {
            // set properties accordingly
            OrderCode = request.OrderDto.OrderCode,
            AssignedSystemAdminId = request.OrderDto.AssignedSystemAdminId.ToString(),
            OrganizationId = request.OrderDto.OrganizationId.ToString()

        };

        // publish message
        await publishEndpoint.Publish(bundleUploadedMessage, cancellationToken);
        // consumers:
        // orders service - to update order status to Finished,
        // notification service - to notify,
        // accounts service - to update number of pending orders for assigned system admin
    }


}
