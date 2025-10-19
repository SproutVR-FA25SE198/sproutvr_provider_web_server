using Common.Application.Contracts.Accounts;
using MassTransit;
using Services.Notifications.Application.Abstractions.Services;
using Services.Notifications.Application.BusinessLogics.Emails;

namespace Services.Notifications.Presentation.Consumers;

#pragma warning disable CA1515 // Consider making public types internal
public class OrganizationRegisterRequestRejectedConsumer : IConsumer<OrganizationRegisterRequestRejectedMessage>
#pragma warning restore CA1515 // Consider making public types internal
{
    private readonly IEmailService _emailService;
    private readonly ILogger<OrganizationRegisterRequestRejectedConsumer> _logger;

    public OrganizationRegisterRequestRejectedConsumer(
        IEmailService emailService,
        ILogger<OrganizationRegisterRequestRejectedConsumer> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrganizationRegisterRequestRejectedMessage> context)
    {
        try
        {
            OrganizationRegisterRequestRejectedMessage message = context.Message;
            
            _logger.LogInformation(
                "Processing organization rejected notification for {OrganizationName} ({Email})", 
                message.Name, 
                message.Email);

            string emailContent = EmailContentHelper.GetOrganizationRejectedEmailHtml(message);

            var emailRequest = new SendEmailRequest
            {
                ToEmail = message.Email,
                Subject = "[SproutVR] Organization Registration Update",
                Body = emailContent
            };

            await _emailService.SendEmailAsync(emailRequest);

            _logger.LogInformation(
                "Successfully sent organization rejected email to {Email}", 
                message.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex, 
                "Failed to send organization rejected email to {Email}", 
                context.Message.Email);
            
            // Publish fault message for retry/monitoring
            await context.Publish(new OrganizationRegisterRequestRejectedFaultMessage
            {
                Name = context.Message.Name,
                Email = context.Message.Email,
                Reason = context.Message.Reason,
                ErrorMessage = ex.Message
            });
            
        }
    }
}
