using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MimeKit;
using Services.Notifications.Application.Abstractions.Services;
using Services.Notifications.Application.BusinessLogics.Emails;
using Services.Notifications.Infrastructure.Helpers;

namespace Services.Notifications.Infrastructure.Services;

internal sealed class EmailService : IEmailService
{
    private readonly EmailSettings _mailSettings;

    public EmailService(IOptions<EmailSettings> mailSettings)
    {
        _mailSettings = mailSettings.Value;
    }
    public async Task SendEmailAsync(SendEmailRequest sendEmailRequest)
    {
        using var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(_mailSettings.Mail));
        email.To.Add(MailboxAddress.Parse(sendEmailRequest.ToEmail));
        email.Subject = sendEmailRequest.Subject;

        var builder = new BodyBuilder();

        if (sendEmailRequest.Attachments != null)
        {
            foreach (IFormFile file in sendEmailRequest.Attachments)
            {
                if (file.Length > 0)
                {
                    using var ms = new MemoryStream();
                    await file.CopyToAsync(ms);
                    ms.Position = 0;

                    await builder.Attachments.AddAsync(
                        file.FileName,
                        ms,
                        ContentType.Parse(file.ContentType)
                    );
                }
            }
        }

        builder.HtmlBody = sendEmailRequest.Body;
        email.Body = builder.ToMessageBody();

        using var smtp = new SmtpClient();

        await smtp.ConnectAsync(
            _mailSettings.Host,
            _mailSettings.Port,
            SecureSocketOptions.StartTls
        );

        await smtp.AuthenticateAsync(
            _mailSettings.Mail,
            _mailSettings.Password
        );

        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}
