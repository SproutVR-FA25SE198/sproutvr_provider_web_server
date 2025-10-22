using Common.Application.Contracts.Orders;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using OrganizationAccountsService;
using Services.Notifications.Application.Abstractions.Grpc;
using Services.Notifications.Application.Abstractions.Services;
using Services.Notifications.Application.BusinessLogics.Emails;
using Services.Notifications.Presentation.Hubs;

namespace Services.Notifications.Presentation.Consumers;

#pragma warning disable CA1515 // Consider making public types internal
public sealed class OrderCreatedConsumer : IConsumer<OrderCreatedMessage>
#pragma warning restore CA1515 // Consider making public types internal
{
    private readonly IGrpcOrganizationClient _grpcOrganizationClient;
    private readonly IGrpcAccountClient _grpcAccountClient;
    private readonly IEmailService _emailService;
    private readonly IHubContext<NotificationHub> _hubContext;
    public OrderCreatedConsumer(IGrpcOrganizationClient grpcOrganizationClient, IGrpcAccountClient grpcAccountClient, IEmailService emailService, IHubContext<NotificationHub> hubContext)
    {
        _grpcOrganizationClient = grpcOrganizationClient;    
        _grpcAccountClient = grpcAccountClient;
        _emailService = emailService;
        _hubContext = hubContext;
    }
    public async Task Consume(ConsumeContext<OrderCreatedMessage> context)
    {
        OrderCreatedMessage message = context.Message;
        // call account service via grpc to get org email
        GetOrganizationByIdResponse organizationResponse = await _grpcOrganizationClient.GetOrganizationByIdAsync(message.OrganizationId);

        // send email invoice for organization customer
        string emailContent = EmailContentHelper.GetOrderCreatedEmailHtml(message);

        var emailRequest = new SendEmailRequest
        {
            ToEmail = organizationResponse.OrganizationEmail,
            Subject = $"[SproutVR] Order Invoice #ORD{message.OrderCode} - Order Confirmed",
            Body = emailContent
        };

        await _emailService.SendEmailAsync(emailRequest);

        // prepare notification content for system admins
        string notificationContent = NotificationContentHelper.OrderCreatedNotification(message, organizationResponse.OrganizationName, organizationResponse.OrganizationEmail);

        // send real-time notification to the assigned system admin via SignalR
        if (message.AssignedSystemAdminId != Guid.Empty)
        {
            // Get system admin information
            Application.BusinessLogics.SystemAdmins.SystemAdminDto? systemAdmin = await _grpcAccountClient.GetSystemAdminByIdAsync(message.AssignedSystemAdminId);
            if (systemAdmin != null)
            {
                // Send notification to specific system admin group
                await _hubContext.Clients.Group($"SystemAdmin_{message.AssignedSystemAdminId}").SendAsync("OrderCreated", notificationContent);
            }
        }

    }
}
