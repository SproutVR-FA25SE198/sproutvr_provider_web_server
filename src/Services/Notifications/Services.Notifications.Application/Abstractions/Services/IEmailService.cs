using Services.Notifications.Application.BusinessLogics.Emails;

namespace Services.Notifications.Application.Abstractions.Services;
public interface IEmailService
{
    Task SendEmailAsync(SendEmailRequest sendEmailRequest);
}
