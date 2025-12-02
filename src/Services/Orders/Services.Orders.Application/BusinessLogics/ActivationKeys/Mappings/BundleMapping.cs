using Services.Orders.Application.BusinessLogics.ActivationKeys.Features.GetBundles;
using Services.Orders.Domain.Entities.OrderItems;
using Services.Orders.Domain.Entities.Orders;

namespace Services.Orders.Application.BusinessLogics.ActivationKeys.Mappings;
public static class BundleMapping
{
    public static BundlePayloadDto ToBundlePayloadDto(this Order order)
    {
        return new BundlePayloadDto
        {
            OrderId = order.Id,
            OrderCode = order.OrderCode,
            OrganizationId = order.OrganizationId,
            MapCount = CountMaps(order),
            Maps = order.OrderItems?.Select(oi => oi.ToMapPayloadDto()).ToList() 
                ?? new List<MapPayloadDto>()
        };
    }

    public static BundlePayloadDto ToBundlePayloadWithoutItemsDto(this Order order)
    {
        return new BundlePayloadDto
        {
            OrderId = order.Id,
            OrderCode = order.OrderCode,
            OrganizationId = order.OrganizationId,
            MapCount = CountMaps(order),
            Maps = new List<MapPayloadDto>()
        };
    }

    public static MapPayloadDto ToMapPayloadDto(this OrderItem orderItem)
    {
        return new MapPayloadDto
        {
            OrderItemId = orderItem.Id,
            MapId = orderItem.MapId,
            MapName = orderItem.MapName,
            MapCode = orderItem.MapCode,
            ImageUrl = orderItem.ImageUrl,
            DownloadUrl = orderItem.DownloadUrl,
            IsDownloaded = orderItem.IsDownloaded,
        };
    }

    /// <summary>
    /// Count the number of maps (order items) in an order
    /// </summary>
    /// <param name="order"></param>
    /// <returns></returns>
    private static int CountMaps(Order order)
    {
        return order.OrderItems.Count;
    }
}
