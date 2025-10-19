using Common.Application.Contracts.Orders;

namespace Services.Notifications.Application.BusinessLogics.Emails;
public static class NotificationContentHelper
{
    public static string OrderCreatedNotification(OrderCreatedMessage orderCreatedMessage, string organizationName, string organizationEmail)
    {
        // notification content for system admin to prepare for new order
        string itemsList = string.Join(", ", orderCreatedMessage.OrderItems.Select(i => i.MapName ?? "Không rõ"));
        int itemsCount = orderCreatedMessage.OrderItems.Count;
        
        return $@"Đơn Hàng Mới #{orderCreatedMessage.OrderCode} - Yêu Cầu Xử Lý!

                📦 Chi Tiết Đơn Hàng:
                   • Tổng tiền: {orderCreatedMessage.TotalMoneyAmount:N0} VND
                   • Sản phẩm: {itemsCount} bản đồ - {itemsList}
                   • Tạo lúc: {orderCreatedMessage.CreatedAtUtc:dd/MM/yyyy HH:mm} UTC

                👤 Liên Hệ:
                   • Tên tổ chức: {organizationName}
                   • Email tổ chức: {organizationEmail}
                   • Người đại diện: {orderCreatedMessage.RepresentativeName}
                   • Số điện thoại: {orderCreatedMessage.RepresentativePhone}

                ⚡ Hành Động: Vui lòng chuẩn bị gói nội dung VR.";
    }
}
