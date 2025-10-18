using Common.Application.Contracts.Accounts;
using MassTransit;
using Services.Notifications.Application.Abstractions.Services;
using Services.Notifications.Application.BusinessLogics.Emails;

namespace Services.Notifications.Presentation.Consumers;

#pragma warning disable CA1515 // Consider making public types internal
public class OrganizationRegisterRequestApprovedConsumer : IConsumer<OrganizationRegisterRequestApprovedMessage>
#pragma warning restore CA1515 // Consider making public types internal
{
    private readonly IEmailService _emailService;
    private readonly ILogger<OrganizationRegisterRequestApprovedConsumer> _logger;

    public OrganizationRegisterRequestApprovedConsumer(
        IEmailService emailService,
        ILogger<OrganizationRegisterRequestApprovedConsumer> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrganizationRegisterRequestApprovedMessage> context)
    {
        try
        {
            OrganizationRegisterRequestApprovedMessage message = context.Message;
            
            _logger.LogInformation(
                "Processing organization approved notification for {OrganizationName} ({Email})", 
                message.Name, 
                message.Email);

            string emailContent = EmailContentHelper.GetOrganizationApprovedEmailHtml(message);

            var emailRequest = new SendEmailRequest
            {
                ToEmail = message.Email,
                Subject = "[SproutVR] Organization Registration Has Been Approved!",
                Body = emailContent
            };

            await _emailService.SendEmailAsync(emailRequest);

            _logger.LogInformation(
                "Successfully sent organization approved email to {Email}", 
                message.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex, 
                "Failed to send organization approved email to {Email}", 
                context.Message.Email);
            
            // Publish fault message for retry/monitoring
            await context.Publish(new OrganizationRegisterRequestApprovedFaultMessage
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
