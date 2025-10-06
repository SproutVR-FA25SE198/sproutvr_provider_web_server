using MediatR;
using Services.Baskets.Domain.Entities;

namespace Services.Baskets.Application.BusinessLogics.Baskets.GetBasketByOrganizationId;
public class GetBasketByOrganizationIdQuery : IRequest<Basket>
{
    public string OrganizationId { get; }
    public GetBasketByOrganizationIdQuery(string organizationId)
    {
        OrganizationId = organizationId;
    }
}
