using MediatR;

namespace Services.Accounts.Application.BusinessLogics.Organizations.Features.GetOrganizationById;
public class GetOrganizationByIdQuery(Guid id) : IRequest<OrganizationDetailsDto>
{
    public Guid Id { get; set; } = id;
}
