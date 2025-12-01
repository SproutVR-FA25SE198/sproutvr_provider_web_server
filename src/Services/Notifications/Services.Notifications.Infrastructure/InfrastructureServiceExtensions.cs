 using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Services.Notifications.Application.Abstractions.Grpc;
using Services.Notifications.Application.Abstractions.Repositories;
using Services.Notifications.Application.Abstractions.Services;
using Services.Notifications.Infrastructure.Data;
using Services.Notifications.Infrastructure.Jobs;
using Services.Notifications.Infrastructure.Repositories;
using Services.Notifications.Infrastructure.Services;
using Services.Notifications.Infrastructure.Services.Grpc;

namespace Services.Notifications.Infrastructure;
public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        // Add MongoDB
        services.AddSingleton<MongoDbContext>();
        services.AddScoped<INotificationRepository, NotificationRepository>();

        // Add User defined Services
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<INotificationService, NotificationService>();

        // Add Quazt
        services.AddQuartz(conf =>
        {
            // Add Email Job
            conf.AddJob<EmailJob>(c =>
                c.WithIdentity(new JobKey(nameof(EmailJob))).StoreDurably());

        }).AddQuartzHostedService(otp =>
        {
            otp.WaitForJobsToComplete = true;
        });

        // Add Grpc
        services.AddScoped<IGrpcOrganizationClient, GrpcOrganizationClient>();
        services.AddScoped<IGrpcAccountClient, GrpcAccountClient>();


        return services;
    }
}
