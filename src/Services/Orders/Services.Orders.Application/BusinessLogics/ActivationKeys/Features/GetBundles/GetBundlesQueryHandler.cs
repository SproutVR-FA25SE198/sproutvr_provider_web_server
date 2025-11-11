using Common.Application.Abstractions.Data;
using Common.Domain.Exceptions;
using MediatR;
using Services.Orders.Application.BusinessLogics.ActivationKeys.Mappings;
using Services.Orders.Application.BusinessLogics.Orders.Specifications;
using Services.Orders.Domain.Entities.Orders;

namespace Services.Orders.Application.BusinessLogics.ActivationKeys.Features.GetBundles;
public class GetBundlesQueryHandler(
    IUnitOfWork uow) : IRequestHandler<GetBundlesQuery, List<BundlePayloadDto>>
{
    public async Task<List<BundlePayloadDto>> Handle(GetBundlesQuery request, CancellationToken cancellationToken)
    {
        // Check if request is null
        if (request.GetBundlesRequestDto == null)
        {
            throw new OperationFailedException("Request payload is null");
        }

        // Check if orgID is null
        if (request.GetBundlesRequestDto.OrganizationId == Guid.Empty)
        {
            throw new OperationFailedException("Không tìm thấy tổ chức nào");
        }

        // Get all key activated order by the requested organization
        var spec = new OrderSpecification(request.GetBundlesRequestDto.OrganizationId, byOrgId: true);
        IReadOnlyList<Order> orders = await uow.Repository<Order>().ListAsync(spec);

        // If no activated bundles are found, return an empty list.
        // This is a valid state, not an error.
        if (orders == null || !orders.Any())
        {
            return new List<BundlePayloadDto>();
        }

        // Map the list of Order entities to a list of BundlePayloadDto
        var bundlePayloads = new List<BundlePayloadDto>();
        foreach (Order order in orders)
        {
            bundlePayloads.Add(order.ToBundlePayloadDto());
        }

        // Return the list of all found bundles
        return bundlePayloads;
    }
}
