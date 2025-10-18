using Common.Application.Contracts.Accounts;
using Common.Application.Contracts.Orders;

namespace Services.Notifications.Application.BusinessLogics.Emails;

public static class EmailContentHelper
{
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
                            <h1 style=""color: #ffffff; margin: 0; font-size: 28px; font-weight: bold;"">🎉 Congratulations!</h1>
                        </td>
                    </tr>
                    
                    <!-- Body -->
                    <tr>
                        <td style=""padding: 40px 30px;"">
                            <h2 style=""color: #333333; margin: 0 0 20px 0; font-size: 24px;"">Dear {message.Name},</h2>
                            
                            <p style=""color: #666666; font-size: 16px; line-height: 24px; margin: 0 0 20px 0;"">
                                We are pleased to inform you that your organization registration request has been <strong style=""color: #10b981;"">approved</strong>!
                            </p>
                            
                            <p style=""color: #666666; font-size: 16px; line-height: 24px; margin: 0 0 30px 0;"">
                                Your account has been created and you can now access our platform using the credentials below:
                            </p>
                            
                            <!-- Credentials Box -->
                            <table role=""presentation"" style=""width: 100%; border-collapse: collapse; background-color: #f8f9fa; border-radius: 8px; margin: 0 0 30px 0;"">
                                <tr>
                                    <td style=""padding: 20px;"">
                                        <table role=""presentation"" style=""width: 100%;"">
                                            <tr>
                                                <td style=""color: #666666; font-size: 14px; padding: 8px 0;"">
                                                    <strong style=""color: #333333;"">Username:</strong>
                                                </td>
                                                <td style=""color: #333333; font-size: 14px; padding: 8px 0; text-align: right;"">
                                                    <code style=""background-color: #e9ecef; padding: 4px 8px; border-radius: 4px; font-family: 'Courier New', monospace;"">{message.UserName}</code>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style=""color: #666666; font-size: 14px; padding: 8px 0;"">
                                                    <strong style=""color: #333333;"">Password:</strong>
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
                                            <strong>⚠️ Important:</strong> Please change your password after your first login for security purposes.
                                        </p>
                                    </td>
                                </tr>
                            </table>
                            
                            <p style=""color: #666666; font-size: 16px; line-height: 24px; margin: 0 0 30px 0;"">
                                If you have any questions or need assistance, please don't hesitate to contact our support team.
                            </p>
                            
                            <!-- CTA Button -->
                            <table role=""presentation"" style=""width: 100%;"">
                                <tr>
                                    <td style=""text-align: center; padding: 20px 0;"">
                                        <a href=""#"" style=""display: inline-block; padding: 14px 40px; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: #ffffff; text-decoration: none; border-radius: 6px; font-weight: bold; font-size: 16px;"">Login to Your Account</a>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    
                    <!-- Footer -->
                    <tr>
                        <td style=""background-color: #f8f9fa; padding: 30px; text-align: center; border-radius: 0 0 8px 8px;"">
                            <p style=""color: #999999; font-size: 14px; line-height: 20px; margin: 0 0 10px 0;"">
                                Best regards,<br>
                                <strong style=""color: #666666;"">SproutVR Team</strong>
                            </p>
                            <p style=""color: #999999; font-size: 12px; margin: 0;"">
                                This is an automated email. Please do not reply to this message.
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
                            <h1 style=""color: #ffffff; margin: 0; font-size: 28px; font-weight: bold;"">Registration Update</h1>
                        </td>
                    </tr>
                    
                    <!-- Body -->
                    <tr>
                        <td style=""padding: 40px 30px;"">
                            <h2 style=""color: #333333; margin: 0 0 20px 0; font-size: 24px;"">Dear {message.Name},</h2>
                            
                            <p style=""color: #666666; font-size: 16px; line-height: 24px; margin: 0 0 20px 0;"">
                                Thank you for your interest in joining our platform. We have carefully reviewed your organization registration request.
                            </p>
                            
                            <p style=""color: #666666; font-size: 16px; line-height: 24px; margin: 0 0 30px 0;"">
                                Unfortunately, we are unable to approve your registration at this time.
                            </p>
                            
                            <!-- Reason Box -->
                            {(!string.IsNullOrEmpty(message.Reason) ? $@"
                            <table role=""presentation"" style=""width: 100%; border-collapse: collapse; background-color: #fef2f2; border-left: 4px solid #ef4444; margin: 0 0 30px 0;"">
                                <tr>
                                    <td style=""padding: 20px;"">
                                        <p style=""color: #666666; font-size: 14px; margin: 0 0 8px 0;"">
                                            <strong style=""color: #991b1b;"">Reason for rejection:</strong>
                                        </p>
                                        <p style=""color: #991b1b; font-size: 15px; line-height: 22px; margin: 0;"">
                                            {message.Reason}
                                        </p>
                                    </td>
                                </tr>
                            </table>
                            " : "")}
                            
                            <p style=""color: #666666; font-size: 16px; line-height: 24px; margin: 0 0 20px 0;"">
                                We understand this may be disappointing news. If you believe there has been an error or if you would like to discuss this decision further, please feel free to contact our support team.
                            </p>
                            
                            <p style=""color: #666666; font-size: 16px; line-height: 24px; margin: 0 0 30px 0;"">
                                You are welcome to submit a new registration request after addressing the concerns mentioned above.
                            </p>
                            
                            <!-- Info Box -->
                            <table role=""presentation"" style=""width: 100%; border-collapse: collapse; background-color: #f0f9ff; border-left: 4px solid #3b82f6; margin: 0 0 30px 0;"">
                                <tr>
                                    <td style=""padding: 15px 20px;"">
                                        <p style=""color: #1e40af; font-size: 14px; line-height: 20px; margin: 0;"">
                                            <strong>💡 Need Help?</strong> Contact our support team at support@sproutvr.com for assistance with your registration.
                                        </p>
                                    </td>
                                </tr>
                            </table>
                            
                            <!-- CTA Button -->
                            <table role=""presentation"" style=""width: 100%;"">
                                <tr>
                                    <td style=""text-align: center; padding: 20px 0;"">
                                        <a href=""#"" style=""display: inline-block; padding: 14px 40px; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: #ffffff; text-decoration: none; border-radius: 6px; font-weight: bold; font-size: 16px;"">Contact Support</a>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    
                    <!-- Footer -->
                    <tr>
                        <td style=""background-color: #f8f9fa; padding: 30px; text-align: center; border-radius: 0 0 8px 8px;"">
                            <p style=""color: #999999; font-size: 14px; line-height: 20px; margin: 0 0 10px 0;"">
                                Best regards,<br>
                                <strong style=""color: #666666;"">SproutVR Team</strong>
                            </p>
                            <p style=""color: #999999; font-size: 12px; margin: 0;"">
                                This is an automated email. Please do not reply to this message.
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
                                                            <strong style=""color: #333333; font-size: 15px; display: block; margin-bottom: 4px;"">{item.MapName ?? "N/A"}</strong>
                                                            <span style=""color: #9ca3af; font-size: 13px;"">Code: {item.MapCode ?? "N/A"}</span>
                                                            {(!string.IsNullOrEmpty(item.SubjectName) ? $@"<br/><span style=""color: #9ca3af; font-size: 13px;"">Subject: {item.SubjectName}</span>" : "")}
                                                        </div>
                                                    </div>
                                                </td>
                                                <td style=""padding: 15px 10px; border-bottom: 1px solid #e5e7eb; text-align: right;"">
                                                    <strong style=""color: #333333; font-size: 16px;"">${item.Price?.ToString("N2") ?? "0.00"}</strong>
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
                                        <p style=""color: #d1fae5; margin: 0; font-size: 16px;"">Order Invoice</p>
                                    </td>
                                    <td style=""text-align: right;"">
                                        <div style=""background-color: rgba(255,255,255,0.2); padding: 10px 20px; border-radius: 6px; display: inline-block;"">
                                            <p style=""color: #ffffff; margin: 0; font-size: 14px;"">Order #</p>
                                            <p style=""color: #ffffff; margin: 5px 0 0 0; font-size: 24px; font-weight: bold;"">{message.OrderCode?.ToString() ?? "N/A"}</p>
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
                                        <h3 style=""color: #333333; margin: 0 0 15px 0; font-size: 16px; font-weight: bold; text-transform: uppercase; letter-spacing: 0.5px;"">Order Details</h3>
                                        <p style=""color: #666666; margin: 0 0 8px 0; font-size: 14px; line-height: 22px;"">
                                            <strong style=""color: #333333;"">Date:</strong> {message.CreatedAtUtc.ToString("MMMM dd, yyyy")}
                                        </p>
                                        <p style=""color: #666666; margin: 0 0 8px 0; font-size: 14px; line-height: 22px;"">
                                            <strong style=""color: #333333;"">Time:</strong> {message.CreatedAtUtc.ToString("hh:mm tt")} UTC
                                        </p>
                                    </td>
                                    <td style=""width: 50%; vertical-align: top;"">
                                        <h3 style=""color: #333333; margin: 0 0 15px 0; font-size: 16px; font-weight: bold; text-transform: uppercase; letter-spacing: 0.5px;"">Contact Information</h3>
                                        <p style=""color: #666666; margin: 0 0 8px 0; font-size: 14px; line-height: 22px;"">
                                            <strong style=""color: #333333;"">Representative:</strong> {message.RepresentativeName}
                                        </p>
                                        <p style=""color: #666666; margin: 0 0 8px 0; font-size: 14px; line-height: 22px;"">
                                            <strong style=""color: #333333;"">Phone:</strong> {message.RepresentativePhone}
                                        </p>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    
                    <!-- Order Items -->
                    <tr>
                        <td style=""padding: 20px 30px;"">
                            <h3 style=""color: #333333; margin: 0 0 20px 0; font-size: 18px; font-weight: bold;"">Order Items</h3>
                            <table role=""presentation"" style=""width: 100%; border-collapse: collapse; border: 1px solid #e5e7eb; border-radius: 8px;"">
                                <thead>
                                    <tr style=""background-color: #f9fafb;"">
                                        <th style=""padding: 15px 10px; text-align: left; color: #6b7280; font-size: 14px; font-weight: 600; text-transform: uppercase; letter-spacing: 0.5px; border-bottom: 2px solid #e5e7eb;"">Item</th>
                                        <th style=""padding: 15px 10px; text-align: right; color: #6b7280; font-size: 14px; font-weight: 600; text-transform: uppercase; letter-spacing: 0.5px; border-bottom: 2px solid #e5e7eb;"">Price</th>
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
                                            Subtotal: <strong style=""color: #333333;"">${message.TotalMoneyAmount.ToString("N2")}</strong>
                                        </p>
                                        <p style=""color: #333333; margin: 0; font-size: 24px; font-weight: bold;"">
                                            Total: <span style=""color: #10b981;"">${message.TotalMoneyAmount.ToString("N2")}</span>
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
                                            <strong>✓ Order Confirmed</strong><br/>
                                            Thank you for your order! We are processing your request and will prepare your VR content bundle shortly. You will receive another notification once your order is ready for delivery.
                                        </p>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    
                    <!-- Next Steps -->
                    <tr>
                        <td style=""padding: 0 30px 40px 30px;"">
                            <h3 style=""color: #333333; margin: 0 0 15px 0; font-size: 18px; font-weight: bold;"">What's Next?</h3>
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
                                                        <strong style=""color: #333333;"">Bundle Preparation:</strong> Our team will prepare your VR content bundle.
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
                                                        <strong style=""color: #333333;"">Quality Check:</strong> We'll ensure all content meets our quality standards.
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
                                                        <strong style=""color: #333333;"">Delivery:</strong> You'll receive access to your VR content bundle.
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
                                If you have any questions about your order, please contact our support team.
                            </p>
                            <p style=""color: #999999; font-size: 14px; line-height: 20px; margin: 0 0 10px 0;"">
                                Best regards,<br>
                                <strong style=""color: #666666;"">SproutVR Team</strong>
                            </p>
                            <p style=""color: #999999; font-size: 12px; margin: 0;"">
                                This is an automated email. Please do not reply to this message.
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

