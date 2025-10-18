using Common.Application.Contracts.Orders;

namespace Services.Notifications.Application.BusinessLogics.Emails;
public static class NotificationContentHelper
{
    public static string OrderCreatedNotification(OrderCreatedMessage orderCreatedMessage)
    {
        // notification content for system admin to prepare for new order
        string itemsList = string.Join(", ", orderCreatedMessage.OrderItems.Select(i => i.MapName ?? "Unknown"));
        int itemsCount = orderCreatedMessage.OrderItems.Count;
        
        return $@"New Order #{orderCreatedMessage.OrderCode} - Action Required!

                📦 Order Details:
                   • Total Amount: ${orderCreatedMessage.TotalMoneyAmount:N2}
                   • Items: {itemsCount} map{(itemsCount != 1 ? "s" : "")} - {itemsList}
                   • Created: {orderCreatedMessage.CreatedAtUtc:MMM dd, yyyy HH:mm} UTC

                👤 Contact:
                   • Representative: {orderCreatedMessage.RepresentativeName}
                   • Phone: {orderCreatedMessage.RepresentativePhone}

                ⚡ Action: Please prepare the VR content bundle for delivery.";
    }
}
