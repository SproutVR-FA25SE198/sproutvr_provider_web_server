using Common.Application.Contracts.Orders;
using Services.Notifications.Application.Helpers;

namespace Services.Notifications.Application.BusinessLogics.Emails;
public static class NotificationContentHelper
{
    public static string OrderCreatedNotification(OrderCreatedMessage orderCreatedMessage, string organizationName, string organizationEmail)
    {
        // notification content for system admin to prepare for new order
        string itemsList = string.Join(", ", orderCreatedMessage.OrderItems.Select(i => i.MapName ?? "Không rõ"));
        int itemsCount = orderCreatedMessage.OrderItems.Count;

        string formattedMoney = EmailUtils.FormatMoney(orderCreatedMessage.TotalMoneyAmount);
        
        #pragma warning disable CA1305 // Specify IFormatProvider
        string formattedTime = EmailUtils.FormatTime(orderCreatedMessage.CreatedAtUtc).ToString("dd/MM/yyyy HH:mm");
        #pragma warning restore CA1305 // Specify IFormatProvider

        return $@"Đơn Hàng Mới #{orderCreatedMessage.OrderCode} - Yêu Cầu Xử Lý!

                📦 Chi Tiết Đơn Hàng:
                   • Tổng tiền: {formattedMoney} VND
                   • Sản phẩm: {itemsCount} bản đồ - {itemsList}
                   • Tạo lúc: {formattedTime} UTC

                👤 Liên Hệ:
                   • Tên tổ chức: {organizationName}
                   • Email tổ chức: {organizationEmail}
                   • Người đại diện: {orderCreatedMessage.RepresentativeName}
                   • Số điện thoại: {orderCreatedMessage.RepresentativePhone}

                ⚡ Hành Động: Vui lòng chuẩn bị gói nội dung VR.";
    }
}
