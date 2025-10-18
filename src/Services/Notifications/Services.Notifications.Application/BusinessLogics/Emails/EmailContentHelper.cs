using Common.Application.Contracts.Accounts;

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
}

