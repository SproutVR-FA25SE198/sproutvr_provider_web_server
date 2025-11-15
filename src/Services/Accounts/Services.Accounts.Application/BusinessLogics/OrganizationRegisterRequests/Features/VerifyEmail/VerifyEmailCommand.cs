using MediatR;

namespace Services.Accounts.Application.BusinessLogics.OrganizationRegisterRequests.Features.VerifyEmail;

public class VerifyEmailCommand : IRequest<bool>
{
    public Guid OrganizationRegisterRequestId { get; set; }
    public string Token { get; set; } = string.Empty;
}

