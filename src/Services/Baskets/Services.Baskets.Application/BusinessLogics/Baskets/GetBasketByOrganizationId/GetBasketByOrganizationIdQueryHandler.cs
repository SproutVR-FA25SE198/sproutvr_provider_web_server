using MediatR;
using Services.Baskets.Application.Abstractions.Data;
using Services.Baskets.Domain.Entities;

namespace Services.Baskets.Application.BusinessLogics.Baskets.GetBasketByOrganizationId;
public class GetBasketByOrganizationIdQueryHandler(IBasketRepository repo) : IRequestHandler<GetBasketByOrganizationIdQuery, Basket>
{
    public async Task<Basket> Handle(GetBasketByOrganizationIdQuery request, CancellationToken cancellationToken)
    {
        Basket basket = await repo.GetBasketByOrganizationIdAsync(request.OrganizationId);

        return basket ?? new Basket() { OrganizationId = Guid.Parse(request.OrganizationId) };
    }
}
