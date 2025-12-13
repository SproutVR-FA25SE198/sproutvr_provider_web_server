using MediatR;

namespace Services.Accounts.Application.BusinessLogics.Organizations.Features.DeactivateOrganization;

public class DeactivateOrganizationCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}

