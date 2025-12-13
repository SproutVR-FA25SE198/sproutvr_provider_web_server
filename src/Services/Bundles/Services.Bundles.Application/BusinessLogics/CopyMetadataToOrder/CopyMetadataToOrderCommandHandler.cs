using MediatR;
using Microsoft.Extensions.Logging;
using OrdersService;
using Services.Bundles.Application.Abstractions.Services;

namespace Services.Bundles.Application.BusinessLogics.CopyMetadataToOrder;

public class CopyMetadataToOrderCommandHandler(
    IGoogleDriveService googleDriveService,
    GrpcOrder.GrpcOrderClient grpcOrderClient,
    ILogger<CopyMetadataToOrderCommandHandler> logger) : IRequestHandler<CopyMetadataToOrderCommand, bool>
{
    public async Task<bool> Handle(CopyMetadataToOrderCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting metadata copy process for Order {OrderId}", request.OrderId);

        // --- 1. Get order items from Order service via gRPC ---
        var orderItemsRequest = new GetOrderItemsRequest
        {
            OrderId = request.OrderId.ToString()
        };

        GetOrderItemsResponse orderItemsResponse = await grpcOrderClient.GetOrderItemsAsync(
            orderItemsRequest, 
            cancellationToken: cancellationToken);

        if (orderItemsResponse.OrderItems.Count == 0)
        {
            logger.LogWarning("No order items found for Order {OrderId}", request.OrderId);
            throw new InvalidOperationException("No order items found for this order");
        }

        logger.LogInformation("Found {Count} order items for Order {OrderId}", 
            orderItemsResponse.OrderItems.Count, 
            request.OrderId);

        // --- 2. Get Google Drive service ---
        Google.Apis.Drive.v3.DriveService driveService = googleDriveService.GetDriveService();

        // --- 3. Copy metadata files and update each order item with its own download link ---
        int successCount = 0;
        int failCount = 0;

        foreach (OrderItemModel? orderItem in orderItemsResponse.OrderItems)
        {
            try
            {
                await CopyMetadataFile(
                    orderItem, 
                    driveService, 
                    request.BundleGoogleDriveFolderId, 
                    cancellationToken);
                
                successCount++;
                logger.LogInformation("Successfully copied metadata for order item {MapCode}", orderItem.MapCode);
            }
            catch (Exception ex)
            {
                failCount++;
                logger.LogError(ex, "Failed to copy metadata for order item {MapCode}", orderItem.MapCode);
                // Continue with next item instead of throwing
            }
        }

        logger.LogInformation(
            "Metadata copy completed for Order {OrderId}. Success: {Success}, Failed: {Failed}", 
            request.OrderId, 
            successCount, 
            failCount);

        // Return true if at least one item was successful
        return successCount > 0;
    }

    private async Task CopyMetadataFile(
        OrderItemModel orderItem,
        Google.Apis.Drive.v3.DriveService driveService,
        string bundleFolderId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(orderItem.DownloadUrl))
        {
            logger.LogWarning("Order item {MapCode} has no download URL, skipping", orderItem.MapCode);
            return;
        }

        // Extract file ID from download URL (storage path)
        System.Text.RegularExpressions.Match fileIdMatch = System.Text.RegularExpressions.Regex.Match(
            orderItem.DownloadUrl,
            @"(?:id=|file/d/)([^/&\?]+)");

        if (!fileIdMatch.Success)
        {
            logger.LogWarning("Could not extract file ID from download URL for {MapCode}", orderItem.MapCode);
            return;
        }

        string mapFileId = fileIdMatch.Groups[1].Value;
        string newFileName = $"{orderItem.MapCode}.zip";

        logger.LogInformation("Copying file {MapCode} (ID: {FileId}) to bundle folder", 
            orderItem.MapCode, 
            mapFileId);

        // Copy the map zip file to the bundle folder
        string copiedFileId = await googleDriveService.CopyFileToFolder(
            driveService, 
            mapFileId, 
            bundleFolderId, 
            newFileName);

        // Get shareable link
        string fileShareableLink = await googleDriveService.GetShareableLinkForFile(driveService, copiedFileId);
        string fileDownloadLink = googleDriveService.ConvertToDirectDownloadLink(fileShareableLink);

        logger.LogInformation("Generated download link for {MapCode}: {Link}", 
            orderItem.MapCode, 
            fileDownloadLink);

        // Update this order item with its specific download link
        var updateItemRequest = new UpdateOrderItemDownloadUrlRequest
        {
            OrderItemId = orderItem.Id,
            NewDownloadUrl = fileDownloadLink
        };

        UpdateOrderItemDownloadUrlResponse updateItemResponse = await grpcOrderClient.UpdateOrderItemDownloadUrlAsync(
            updateItemRequest, 
            cancellationToken: cancellationToken);

        if (!updateItemResponse.IsSuccess)
        {
            throw new InvalidOperationException(
                $"Failed to update order item {orderItem.MapCode}: {updateItemResponse.Message}");
        }

        logger.LogInformation("Successfully updated download URL for order item {MapCode}", orderItem.MapCode);
    }
}

