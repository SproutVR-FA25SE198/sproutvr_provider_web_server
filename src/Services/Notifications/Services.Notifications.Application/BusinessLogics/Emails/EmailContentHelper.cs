using Common.Application.Contracts.Accounts;
using Common.Application.Contracts.Orders;
using Services.Notifications.Application.Helpers;

namespace Services.Notifications.Application.BusinessLogics.Emails;

public static class EmailContentHelper
{
    public static string GetOrganizationCreatedEmailHtml(OrganizationCreatedMessage message)
    {
        return $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Tài Khoản Tổ Chức Mới</title>
</head>
<body style=""margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #f4f4f4;"">
    <table role=""presentation"" style=""width: 100%; border-collapse: collapse;"">
        <tr>
            <td style=""padding: 20px 0;"">
                <table role=""presentation"" style=""width: 600px; margin: 0 auto; background-color: #ffffff; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.1);"">
                    <!-- Header -->
                    <tr>
                        <td style=""background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 40px 30px; text-align: center; border-radius: 8px 8px 0 0;"">
                            <h1 style=""color: #ffffff; margin: 0; font-size: 28px; font-weight: bold;"">Chào Mừng Đến Với SproutVR!</h1>
                        </td>
                    </tr>
                    
                    <!-- Body -->
                    <tr>
                        <td style=""padding: 40px 30px;"">
                            <h2 style=""color: #333333; margin: 0 0 20px 0; font-size: 24px;"">Kính gửi {message.Name},</h2>
                            
                            <p style=""color: #666666; font-size: 16px; line-height: 24px; margin: 0 0 20px 0;"">
                                Tài khoản tổ chức của bạn đã được <strong style=""color: #10b981;"">tạo thành công</strong> trên nền tảng SproutVR!
                            </p>
                            
                            <p style=""color: #666666; font-size: 16px; line-height: 24px; margin: 0 0 30px 0;"">
                                Bạn có thể bắt đầu sử dụng nền tảng của chúng tôi bằng thông tin đăng nhập dưới đây:
                            </p>
                            
                            <!-- Credentials Box -->
                            <table role=""presentation"" style=""width: 100%; border-collapse: collapse; background-color: #f8f9fa; border-radius: 8px; margin: 0 0 30px 0;"">
                                <tr>
                                    <td style=""padding: 20px;"">
                                        <table role=""presentation"" style=""width: 100%;"">
                                            <tr>
                                                <td style=""color: #666666; font-size: 14px; padding: 8px 0;"">
                                                    <strong style=""color: #333333;"">Tên đăng nhập:</strong>
                                                </td>
                                                <td style=""color: #333333; font-size: 14px; padding: 8px 0; text-align: right;"">
                                                    <code style=""background-color: #e9ecef; padding: 4px 8px; border-radius: 4px; font-family: 'Courier New', monospace;"">{message.UserName}</code>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style=""color: #666666; font-size: 14px; padding: 8px 0;"">
                                                    <strong style=""color: #333333;"">Mật khẩu:</strong>
                                                </td>
                                                <td style=""color: #333333; font-size: 14px; padding: 8px 0; text-align: right;"">
                                                    <code style=""background-color: #e9ecef; padding: 4px 8px; border-radius: 4px; font-family: 'Courier New', monospace;"">{message.Password}</code>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            
                            <!-- Security Notice -->
                            <table role=""presentation"" style=""width: 100%; border-collapse: collapse; background-color: #fff3cd; border-left: 4px solid #ffc107; margin: 0 0 30px 0;"">
                                <tr>
                                    <td style=""padding: 15px 20px;"">
                                        <p style=""color: #856404; font-size: 14px; line-height: 20px; margin: 0;"">
                                            <strong>⚠️ Quan trọng:</strong> Vui lòng thay đổi mật khẩu ngay sau lần đăng nhập đầu tiên để đảm bảo an toàn cho tài khoản của bạn.
                                        </p>
                                    </td>
                                </tr>
                            </table>
                            
                            <p style=""color: #666666; font-size: 16px; line-height: 24px; margin: 0 0 30px 0;"">
                                Nếu bạn có bất kỳ câu hỏi nào hoặc cần hỗ trợ, vui lòng liên hệ với đội ngũ hỗ trợ của chúng tôi tại hotline 0123654789.
                            </p>
                        </td>
                    </tr>
                    
                    <!-- Footer -->
                    <tr>
                        <td style=""background-color: #f8f9fa; padding: 30px; text-align: center; border-radius: 0 0 8px 8px;"">
                            <p style=""color: #999999; font-size: 14px; line-height: 20px; margin: 0 0 10px 0;"">
                                Trân trọng,<br>
                                <strong style=""color: #666666;"">Đội Ngũ SproutVR</strong>
                            </p>
                            <p style=""color: #999999; font-size: 12px; margin: 0;"">
                                Đây là email tự động. Vui lòng không trả lời tin nhắn này.
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
    }

    public static string GetOrganizationApprovedEmailHtml(OrganizationRegisterRequestApprovedMessage message)
    {
        return $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Organization Approved</title>
</head>
<body style=""margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #f4f4f4;"">
    <table role=""presentation"" style=""width: 100%; border-collapse: collapse;"">
        <tr>
            <td style=""padding: 20px 0;"">
                <table role=""presentation"" style=""width: 600px; margin: 0 auto; background-color: #ffffff; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.1);"">
                    <!-- Header -->
                    <tr>
                        <td style=""background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 40px 30px; text-align: center; border-radius: 8px 8px 0 0;"">
                            <h1 style=""color: #ffffff; margin: 0; font-size: 28px; font-weight: bold;"">Chào mừng đến với SproutVR!</h1>
                        </td>
                    </tr>
                    
                    <!-- Body -->
                    <tr>
                        <td style=""padding: 40px 30px;"">
                            <h2 style=""color: #333333; margin: 0 0 20px 0; font-size: 24px;"">Kính gửi {message.Name},</h2>
                            
                            <p style=""color: #666666; font-size: 16px; line-height: 24px; margin: 0 0 20px 0;"">
                                Chúng tôi vui mừng thông báo rằng yêu cầu đăng ký tổ chức của bạn đã được <strong style=""color: #10b981;"">phê duyệt</strong>!
                            </p>
                            
                            <p style=""color: #666666; font-size: 16px; line-height: 24px; margin: 0 0 30px 0;"">
                                Tài khoản của bạn đã được tạo và bạn có thể truy cập nền tảng của chúng tôi bằng thông tin đăng nhập dưới đây:
                            </p>
                            
                            <!-- Credentials Box -->
                            <table role=""presentation"" style=""width: 100%; border-collapse: collapse; background-color: #f8f9fa; border-radius: 8px; margin: 0 0 30px 0;"">
                                <tr>
                                    <td style=""padding: 20px;"">
                                        <table role=""presentation"" style=""width: 100%;"">
                                            <tr>
                                                <td style=""color: #666666; font-size: 14px; padding: 8px 0;"">
                                                    <strong style=""color: #333333;"">Tên đăng nhập:</strong>
                                                </td>
                                                <td style=""color: #333333; font-size: 14px; padding: 8px 0; text-align: right;"">
                                                    <code style=""background-color: #e9ecef; padding: 4px 8px; border-radius: 4px; font-family: 'Courier New', monospace;"">{message.UserName}</code>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style=""color: #666666; font-size: 14px; padding: 8px 0;"">
                                                    <strong style=""color: #333333;"">Mật khẩu:</strong>
                                                </td>
                                                <td style=""color: #333333; font-size: 14px; padding: 8px 0; text-align: right;"">
                                                    <code style=""background-color: #e9ecef; padding: 4px 8px; border-radius: 4px; font-family: 'Courier New', monospace;"">{message.Password}</code>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            
                            <!-- Security Notice -->
                            <table role=""presentation"" style=""width: 100%; border-collapse: collapse; background-color: #fff3cd; border-left: 4px solid #ffc107; margin: 0 0 30px 0;"">
                                <tr>
                                    <td style=""padding: 15px 20px;"">
                                        <p style=""color: #856404; font-size: 14px; line-height: 20px; margin: 0;"">
                                            <strong>⚠️ Quan trọng:</strong> Vui lòng thay đổi mật khẩu sau lần đăng nhập đầu tiên để đảm bảo an toàn.
                                        </p>
                                    </td>
                                </tr>
                            </table>
                            
                            <p style=""color: #666666; font-size: 16px; line-height: 24px; margin: 0 0 30px 0;"">
                                Nếu bạn có bất kỳ câu hỏi nào hoặc cần hỗ trợ, vui lòng liên hệ với đội ngũ hỗ trợ của chúng tôi.
                            </p>
                            
                        </td>
                    </tr>
                    
                    <!-- Footer -->
                    <tr>
                        <td style=""background-color: #f8f9fa; padding: 30px; text-align: center; border-radius: 0 0 8px 8px;"">
                            <p style=""color: #999999; font-size: 14px; line-height: 20px; margin: 0 0 10px 0;"">
                                Trân trọng,<br>
                                <strong style=""color: #666666;"">Đội Ngũ SproutVR</strong>
                            </p>
                            <p style=""color: #999999; font-size: 12px; margin: 0;"">
                                Đây là email tự động. Vui lòng không trả lời tin nhắn này.
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
    }

    public static string GetOrganizationRejectedEmailHtml(OrganizationRegisterRequestRejectedMessage message)
    {
        return $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Organization Registration Update</title>
</head>
<body style=""margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #f4f4f4;"">
    <table role=""presentation"" style=""width: 100%; border-collapse: collapse;"">
        <tr>
            <td style=""padding: 20px 0;"">
                <table role=""presentation"" style=""width: 600px; margin: 0 auto; background-color: #ffffff; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.1);"">
                    <!-- Header -->
                    <tr>
                        <td style=""background: linear-gradient(135deg, #ef4444 0%, #dc2626 100%); padding: 40px 30px; text-align: center; border-radius: 8px 8px 0 0;"">
                            <h1 style=""color: #ffffff; margin: 0; font-size: 28px; font-weight: bold;"">Cập Nhật Yêu Cầu Đăng Ký</h1>
                        </td>
                    </tr>
                    
                    <!-- Body -->
                    <tr>
                        <td style=""padding: 40px 30px;"">
                            <h2 style=""color: #333333; margin: 0 0 20px 0; font-size: 24px;"">Kính gửi {message.Name},</h2>
                            
                            <p style=""color: #666666; font-size: 16px; line-height: 24px; margin: 0 0 20px 0;"">
                                Cảm ơn bạn đã quan tâm đến nền tảng của chúng tôi. Chúng tôi đã xem xét kỹ lưỡng yêu cầu đăng ký tổ chức của bạn.
                            </p>
                            
                            <p style=""color: #666666; font-size: 16px; line-height: 24px; margin: 0 0 30px 0;"">
                                Rất tiếc, chúng tôi không thể phê duyệt đăng ký của bạn vào thời điểm này.
                            </p>
                            
                            <!-- Reason Box -->
                            {(!string.IsNullOrEmpty(message.Reason) ? $@"
                            <table role=""presentation"" style=""width: 100%; border-collapse: collapse; background-color: #fef2f2; border-left: 4px solid #ef4444; margin: 0 0 30px 0;"">
                                <tr>
                                    <td style=""padding: 20px;"">
                                        <p style=""color: #666666; font-size: 14px; margin: 0 0 8px 0;"">
                                            <strong style=""color: #991b1b;"">Lý do từ chối:</strong>
                                        </p>
                                        <p style=""color: #991b1b; font-size: 15px; line-height: 22px; margin: 0;"">
                                            {message.Reason}
                                        </p>
                                    </td>
                                </tr>
                            </table>
                            " : "")}
                            
                            <p style=""color: #666666; font-size: 16px; line-height: 24px; margin: 0 0 20px 0;"">
                                Nếu bạn cho rằng có sự nhầm lẫn hoặc muốn thảo luận thêm về quyết định này, vui lòng liên hệ với đội ngũ hỗ trợ của chúng tôi.
                            </p>
                            
                            <p style=""color: #666666; font-size: 16px; line-height: 24px; margin: 0 0 30px 0;"">
                                Bạn có thể gửi yêu cầu đăng ký mới sau khi giải quyết các vấn đề được đề cập ở trên.
                            </p>
                            
                            <!-- Info Box -->
                            <table role=""presentation"" style=""width: 100%; border-collapse: collapse; background-color: #f0f9ff; border-left: 4px solid #3b82f6; margin: 0 0 30px 0;"">
                                <tr>
                                    <td style=""padding: 15px 20px;"">
                                        <p style=""color: #1e40af; font-size: 14px; line-height: 20px; margin: 0;"">
                                            <strong>💡 Cần Hỗ Trợ?</strong> Liên hệ đội ngũ hỗ trợ của chúng tôi tại hotline 0123654789 để được hỗ trợ đăng ký.
                                        </p>
                                    </td>
                                </tr>
                            </table>
                            
                            <!-- CTA Button -->
                            <table role=""presentation"" style=""width: 100%;"">
                                <tr>
                                    <td style=""text-align: center; padding: 20px 0;"">
                                        <a href=""#"" style=""display: inline-block; padding: 14px 40px; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: #ffffff; text-decoration: none; border-radius: 6px; font-weight: bold; font-size: 16px;"">Liên Hệ Hỗ Trợ</a>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    
                    <!-- Footer -->
                    <tr>
                        <td style=""background-color: #f8f9fa; padding: 30px; text-align: center; border-radius: 0 0 8px 8px;"">
                            <p style=""color: #999999; font-size: 14px; line-height: 20px; margin: 0 0 10px 0;"">
                                Trân trọng,<br>
                                <strong style=""color: #666666;"">Đội Ngũ SproutVR</strong>
                            </p>
                            <p style=""color: #999999; font-size: 12px; margin: 0;"">
                                Đây là email tự động. Vui lòng không trả lời tin nhắn này.
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
    }

    public static string GetOrderCreatedEmailHtml(OrderCreatedMessage message)
    {
        // invoice email
#pragma warning disable CA1305 // Specify IFormatProvider
        string orderItemsRows = string.Join("", message.OrderItems.Select(static item => $@"
                                            <tr>
                                                <td style=""padding: 15px 10px; border-bottom: 1px solid #e5e7eb;"">
                                                    <div style=""display: flex; align-items: center;"">
                                                        {(!string.IsNullOrEmpty(item.ImageUrl) ? $@"<img src=""{item.ImageUrl}"" alt=""{item.MapName}"" style=""width: 60px; height: 60px; object-fit: cover; border-radius: 6px; margin-right: 15px;""/>" : "")}
                                                        <div>
                                                            <strong style=""color: #333333; font-size: 15px; display: block; margin-bottom: 4px;"">{item.MapName ?? "Không có"}</strong>
                                                            <span style=""color: #9ca3af; font-size: 13px;"">Mã: {item.MapCode ?? "Không có"}</span>
                                                            {(!string.IsNullOrEmpty(item.SubjectName) ? $@"<br/><span style=""color: #9ca3af; font-size: 13px;"">Môn học: {item.SubjectName}</span>" : "")}
                                                        </div>
                                                    </div>
                                                </td>
                                                <td style=""padding: 15px 10px; border-bottom: 1px solid #e5e7eb; text-align: right;"">
                                                    <strong style=""color: #333333; font-size: 16px;"">{EmailUtils.FormatMoney(item.Price)} VND</strong>
                                                </td>
                                            </tr>"));

        return $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Order Invoice</title>
</head>
<body style=""margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #f4f4f4;"">
    <table role=""presentation"" style=""width: 100%; border-collapse: collapse;"">
        <tr>
            <td style=""padding: 20px 0;"">
                <table role=""presentation"" style=""width: 700px; margin: 0 auto; background-color: #ffffff; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.1);"">
                    <!-- Header -->
                    <tr>
                        <td style=""background: linear-gradient(135deg, #10b981 0%, #059669 100%); padding: 40px 30px; border-radius: 8px 8px 0 0;"">
                            <table role=""presentation"" style=""width: 100%;"">
                                <tr>
                                    <td>
                                        <h1 style=""color: #ffffff; margin: 0 0 10px 0; font-size: 32px; font-weight: bold;"">SproutVR</h1>
                                        <p style=""color: #d1fae5; margin: 0; font-size: 16px;"">Hóa Đơn Đặt Hàng</p>
                                    </td>
                                    <td style=""text-align: right;"">
                                        <div style=""background-color: rgba(255,255,255,0.2); padding: 10px 20px; border-radius: 6px; display: inline-block;"">
                                            <p style=""color: #ffffff; margin: 0; font-size: 14px;"">Đơn Hàng </p>
                                            <p style=""color: #ffffff; margin: 5px 0 0 0; font-size: 24px; font-weight: bold;"">#ORD{message.OrderCode?.ToString() ?? "Không có"}</p>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    
                    <!-- Order Info -->
                    <tr>
                        <td style=""padding: 40px 30px 20px 30px;"">
                            <table role=""presentation"" style=""width: 100%;"">
                                <tr>
                                    <td style=""width: 50%; vertical-align: top;"">
                                        <h3 style=""color: #333333; margin: 0 0 15px 0; font-size: 16px; font-weight: bold; text-transform: uppercase; letter-spacing: 0.5px;"">Chi Tiết Đơn Hàng</h3>
                                        <p style=""color: #666666; margin: 0 0 8px 0; font-size: 14px; line-height: 22px;"">
                                            <strong style=""color: #333333;"">Ngày:</strong> {EmailUtils.FormatTime(message.CreatedAtUtc).ToString("dd/MM/yyyy")}
                                        </p>
                                        <p style=""color: #666666; margin: 0 0 8px 0; font-size: 14px; line-height: 22px;"">
                                            <strong style=""color: #333333;"">Thời gian:</strong> {EmailUtils.FormatTime(message.CreatedAtUtc).ToString("HH:mm")} (GMT+7)
                                        </p>
                                    </td>
                                    <td style=""width: 50%; vertical-align: top;"">
                                        <h3 style=""color: #333333; margin: 0 0 15px 0; font-size: 16px; font-weight: bold; text-transform: uppercase; letter-spacing: 0.5px;"">Thông Tin Liên Hệ</h3>
                                        <p style=""color: #666666; margin: 0 0 8px 0; font-size: 14px; line-height: 22px;"">
                                            <strong style=""color: #333333;"">Người đại diện:</strong> {message.RepresentativeName}
                                        </p>
                                        <p style=""color: #666666; margin: 0 0 8px 0; font-size: 14px; line-height: 22px;"">
                                            <strong style=""color: #333333;"">Số điện thoại:</strong> {message.RepresentativePhone}
                                        </p>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    
                    <!-- Order Items -->
                    <tr>
                        <td style=""padding: 20px 30px;"">
                            <h3 style=""color: #333333; margin: 0 0 20px 0; font-size: 18px; font-weight: bold;"">Sản Phẩm</h3>
                            <table role=""presentation"" style=""width: 100%; border-collapse: collapse; border: 1px solid #e5e7eb; border-radius: 8px;"">
                                <thead>
                                    <tr style=""background-color: #f9fafb;"">
                                        <th style=""padding: 15px 10px; text-align: left; color: #6b7280; font-size: 14px; font-weight: 600; text-transform: uppercase; letter-spacing: 0.5px; border-bottom: 2px solid #e5e7eb;"">Sản Phẩm</th>
                                        <th style=""padding: 15px 10px; text-align: right; color: #6b7280; font-size: 14px; font-weight: 600; text-transform: uppercase; letter-spacing: 0.5px; border-bottom: 2px solid #e5e7eb;"">Giá</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {orderItemsRows}
                                </tbody>
                            </table>
                        </td>
                    </tr>
                    
                    <!-- Total -->
                    <tr>
                        <td style=""padding: 20px 30px;"">
                            <table role=""presentation"" style=""width: 100%;"">
                                <tr>
                                    <td style=""text-align: right; padding: 20px 0; border-top: 2px solid #e5e7eb;"">
                                        <p style=""color: #666666; margin: 0 0 10px 0; font-size: 16px;"">
                                            Tạm tính: <strong style=""color: #333333;"">{EmailUtils.FormatMoney(message.TotalMoneyAmount)} VND</strong>
                                        </p>
                                        <p style=""color: #333333; margin: 0; font-size: 24px; font-weight: bold;"">
                                            Tổng cộng: <span style=""color: #10b981;"">{EmailUtils.FormatMoney(message.TotalMoneyAmount)} VND</span>
                                        </p>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    
                    <!-- Status Notice -->
                    <tr>
                        <td style=""padding: 0 30px 30px 30px;"">
                            <table role=""presentation"" style=""width: 100%; border-collapse: collapse; background-color: #ecfdf5; border-left: 4px solid #10b981; border-radius: 6px;"">
                                <tr>
                                    <td style=""padding: 20px;"">
                                        <p style=""color: #065f46; font-size: 15px; line-height: 22px; margin: 0;"">
                                            <strong>✓ Đơn Hàng Đã Xác Nhận</strong><br/>
                                            Cảm ơn bạn đã đặt hàng! Chúng tôi đang xử lý yêu cầu của bạn và sẽ chuẩn bị gói nội dung VR của bạn trong thời gian sớm nhất. Bạn sẽ nhận được thông báo khác khi đơn hàng của bạn đã chuẩn bị xong.
                                        </p>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    
                    <!-- Next Steps -->
                    <tr>
                        <td style=""padding: 0 30px 40px 30px;"">
                            <h3 style=""color: #333333; margin: 0 0 15px 0; font-size: 18px; font-weight: bold;"">Tiếp Theo Là Gì?</h3>
                            <table role=""presentation"" style=""width: 100%;"">
                                <tr>
                                    <td style=""padding: 10px 0;"">
                                        <table role=""presentation"">
                                            <tr>
                                                <td style=""vertical-align: top; padding-right: 15px;"">
                                                    <div style=""width: 30px; height: 30px; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); border-radius: 50%; display: flex; align-items: center; justify-content: center; color: white; font-weight: bold; font-size: 14px;"">1</div>
                                                </td>
                                                <td>
                                                    <p style=""color: #666666; margin: 0; font-size: 15px; line-height: 22px;"">
                                                        <strong style=""color: #333333;"">Chuẩn Bị Gói:</strong> Đội ngũ của chúng tôi sẽ chuẩn bị gói nội dung VR cho bạn.
                                                    </p>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td style=""padding: 10px 0;"">
                                        <table role=""presentation"">
                                            <tr>
                                                <td style=""vertical-align: top; padding-right: 15px;"">
                                                    <div style=""width: 30px; height: 30px; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); border-radius: 50%; display: flex; align-items: center; justify-content: center; color: white; font-weight: bold; font-size: 14px;"">2</div>
                                                </td>
                                                <td>
                                                    <p style=""color: #666666; margin: 0; font-size: 15px; line-height: 22px;"">
                                                        <strong style=""color: #333333;"">Kiểm Tra Chất Lượng:</strong> Chúng tôi sẽ đảm bảo tất cả nội dung đáp ứng tiêu chuẩn chất lượng của chúng tôi.
                                                    </p>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td style=""padding: 10px 0;"">
                                        <table role=""presentation"">
                                            <tr>
                                                <td style=""vertical-align: top; padding-right: 15px;"">
                                                    <div style=""width: 30px; height: 30px; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); border-radius: 50%; display: flex; align-items: center; justify-content: center; color: white; font-weight: bold; font-size: 14px;"">3</div>
                                                </td>
                                                <td>
                                                    <p style=""color: #666666; margin: 0; font-size: 15px; line-height: 22px;"">
                                                        <strong style=""color: #333333;"">Giao Hàng:</strong> Bạn sẽ nhận được quyền truy cập vào gói nội dung VR của mình.
                                                    </p>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    
                    <!-- Footer -->
                    <tr>
                        <td style=""background-color: #f8f9fa; padding: 30px; text-align: center; border-radius: 0 0 8px 8px;"">
                            <p style=""color: #999999; font-size: 14px; line-height: 20px; margin: 0 0 15px 0;"">
                                Nếu bạn có bất kỳ câu hỏi nào về đơn hàng của mình, vui lòng liên hệ với đội ngũ hỗ trợ của chúng tôi.
                            </p>
                            <p style=""color: #999999; font-size: 14px; line-height: 20px; margin: 0 0 10px 0;"">
                                Trân trọng,<br>
                                <strong style=""color: #666666;"">Đội Ngũ SproutVR</strong>
                            </p>
                            <p style=""color: #999999; font-size: 12px; margin: 0;"">
                                Đây là email tự động. Vui lòng không trả lời tin nhắn này.
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
#pragma warning restore CA1305 // Specify IFormatProvider

    }
}

