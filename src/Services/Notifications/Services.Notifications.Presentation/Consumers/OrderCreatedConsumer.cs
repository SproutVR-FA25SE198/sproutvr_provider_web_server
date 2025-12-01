using Common.Application.Contracts.Orders;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using OrganizationAccountsService;
using Services.Notifications.Application.Abstractions.Grpc;
using Services.Notifications.Application.Abstractions.Services;
using Services.Notifications.Application.BusinessLogics.Emails;
using Services.Notifications.Domain.Entities;
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
    private readonly INotificationService _notificationService;
    public OrderCreatedConsumer(IGrpcOrganizationClient grpcOrganizationClient, IGrpcAccountClient grpcAccountClient, IEmailService emailService, IHubContext<NotificationHub> hubContext, INotificationService notificationService)
    {
        _grpcOrganizationClient = grpcOrganizationClient;    
        _grpcAccountClient = grpcAccountClient;
        _emailService = emailService;
        _hubContext = hubContext;
        _notificationService = notificationService;
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
            Subject = $"[SproutVR] Hóa Đơn #ORD{message.OrderCode} - Đơn hàng đã được xác nhận",
            Body = emailContent
        };

        await _emailService.SendEmailAsync(emailRequest);

        // prepare notification content for system admins
        string notificationContent = NotificationContentHelper.OrderCreatedNotification(message, organizationResponse.OrganizationName, organizationResponse.OrganizationEmail);

        // send real-time notification to the assigned system admin via SignalR and save to database
        if (message.AssignedSystemAdminId != Guid.Empty)
        {
            // Get system admin information
            Application.BusinessLogics.SystemAdmins.SystemAdminDto? systemAdmin = await _grpcAccountClient.GetSystemAdminByIdAsync(message.AssignedSystemAdminId);
            if (systemAdmin != null)
            {
                // Send notification to specific system admin group via SignalR
                await _hubContext.Clients.Group($"SystemAdmin_{message.AssignedSystemAdminId}").SendAsync("OrderCreated", notificationContent);

                // Save notification to database
                var notification = new Notification
                {
                    Title = $"Đơn hàng mới #ORD{message.OrderCode}",
                    Content = notificationContent,
                    OrganizationId = message.OrganizationId,
                    AdminId = message.AssignedSystemAdminId.ToString(),
                    Type = "OrderCreated",
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };

                await _notificationService.CreateAsync(notification);
            }
        }

    }
}
