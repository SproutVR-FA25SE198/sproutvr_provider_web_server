using System.IO.Compression;
using Common.Application.Abstractions.Data;
using Common.Application.Contracts.Bundles;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Http;
using OrdersService;
using Services.Bundles.Application.Abstractions.Services;

namespace Services.Bundles.Application.BusinessLogics.UploadBundle;
public class UploadBundleCommandHandler(
    IPublishEndpoint publishEndpoint, 
    IGoogleDriveService googleDriveService,
    GrpcOrder.GrpcOrderClient grpcOrderClient,
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

        // --- 3. Get order items from Order service via gRPC ---
        var orderItemsRequest = new GetOrderItemsRequest
        {
            OrderId = request.OrderDto.OrderId.ToString()
        };
        GetOrderItemsResponse orderItemsResponse = await grpcOrderClient.GetOrderItemsAsync(orderItemsRequest, cancellationToken: cancellationToken);
        
        if (orderItemsResponse.OrderItems.Count == 0)
        {
            throw new InvalidOperationException("No order items found for this order");
        }

        // --- 4. Copy map zip files and update each order item with its own download link ---
        foreach (OrderItemModel? orderItem in orderItemsResponse.OrderItems)
        {
            await CopyMetadataFiles(orderItem, driveService, bundleFolderId, cancellationToken);
        }

        // --- 5. Publish BundleUploaded event ---
        await PublishBundleUploadedEvent(request, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

    }

    // copy metadata files from admin folder to customer bundle folder
    private async Task CopyMetadataFiles(OrderItemModel orderItem, Google.Apis.Drive.v3.DriveService driveService, string bundleFolderId, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(orderItem.DownloadUrl))
        {
            // Extract file ID from download URL (storage path)
            System.Text.RegularExpressions.Match fileIdMatch = System.Text.RegularExpressions.Regex.Match(
                orderItem.DownloadUrl,
                @"(?:id=|file/d/)([^/&\?]+)");

            if (fileIdMatch.Success)
            {
                string mapFileId = fileIdMatch.Groups[1].Value;
                string newFileName = $"{orderItem.MapCode}.zip";

                // Copy the map zip file to the bundle folder
                string copiedFileId = await googleDriveService.CopyFileToFolder(driveService, mapFileId, bundleFolderId, newFileName);

                // Get shareable link
                string fileShareableLink = await googleDriveService.GetShareableLinkForFile(driveService, copiedFileId);
                string fileDownloadLink = googleDriveService.ConvertToDirectDownloadLink(fileShareableLink);

                // Update this order item with its specific download link
                var updateItemRequest = new UpdateOrderItemDownloadUrlRequest
                {
                    OrderItemId = orderItem.Id,
                    NewDownloadUrl = fileDownloadLink
                };

                UpdateOrderItemDownloadUrlResponse updateItemResponse = await grpcOrderClient.UpdateOrderItemDownloadUrlAsync(updateItemRequest, cancellationToken: cancellationToken);

                if (!updateItemResponse.IsSuccess)
                {
                    throw new InvalidOperationException($"Failed to update order item {orderItem.MapCode}: {updateItemResponse.Message}");
                }
            }
        }
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
