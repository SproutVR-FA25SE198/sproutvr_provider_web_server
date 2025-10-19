using Common.Application.Contracts.Accounts;
using MassTransit;
using Services.Notifications.Application.Abstractions.Services;
using Services.Notifications.Application.BusinessLogics.Emails;

namespace Services.Notifications.Presentation.Consumers;

#pragma warning disable CA1515 // Consider making public types internal
public class OrganizationCreatedConsumer : IConsumer<OrganizationCreatedMessage>
#pragma warning restore CA1515 // Consider making public types internal
{
    private readonly IEmailService _emailService;
    private readonly ILogger<OrganizationCreatedConsumer> _logger;

    public OrganizationCreatedConsumer(
        IEmailService emailService,
        ILogger<OrganizationCreatedConsumer> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrganizationCreatedMessage> context)
    {
        try
        {
            OrganizationCreatedMessage message = context.Message;
            
            _logger.LogInformation(
                "Processing organization created notification for {OrganizationName} ({Email})", 
                message.Name, 
                message.Email);

            string emailContent = EmailContentHelper.GetOrganizationCreatedEmailHtml(message);

            var emailRequest = new SendEmailRequest
            {
                ToEmail = message.Email,
                Subject = "[SproutVR] Tài Khoản Tổ Chức Đã Được Tạo",
                Body = emailContent
            };

            await _emailService.SendEmailAsync(emailRequest);

            _logger.LogInformation(
                "Successfully sent organization created email to {Email}", 
                message.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex, 
                "Failed to send organization created email to {Email}", 
                context.Message.Email);
            
            // Publish fault message for retry/monitoring
            await context.Publish(new OrganizationCreatedFaultMessage
            {
                Name = context.Message.Name,
                Email = context.Message.Email,
                OrganizationId = context.Message.OrganizationId,
                UserName = context.Message.UserName,
                Password = context.Message.Password,
                ErrorMessage = ex.Message
            });
        }
    }
}

