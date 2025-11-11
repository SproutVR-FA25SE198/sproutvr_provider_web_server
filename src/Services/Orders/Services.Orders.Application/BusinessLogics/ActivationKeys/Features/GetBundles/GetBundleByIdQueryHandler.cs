using Common.Application.Abstractions.Data;
using Common.Domain.Exceptions;
using MediatR;
using Services.Orders.Application.BusinessLogics.ActivationKeys.Mappings;
using Services.Orders.Application.BusinessLogics.Orders.Specifications;
using Services.Orders.Domain.Entities.Orders;

namespace Services.Orders.Application.BusinessLogics.ActivationKeys.Features.GetBundles;
public class GetBundleByIdQueryHandler(
    IUnitOfWork uow) : IRequestHandler<GetBundleByIdQuery, BundlePayloadDto>
{
    public async Task<BundlePayloadDto> Handle(GetBundleByIdQuery request, CancellationToken cancellationToken)
    {
        // Check if request is null
        if (request.GetBundleByIdRequestDto == null)
        {
            throw new OperationFailedException("Request payload is null");
        }

        // Check if orgID is null
        if (request.GetBundleByIdRequestDto.OrganizationId == Guid.Empty)
        {
            throw new OperationFailedException("Không tìm thấy tổ chức nào");
        }

        // Get order by ID
        var spec = new OrderSpecification(request.OrderId, byOrgId: false);
        Order order = await uow.Repository<Order>().GetEntityWithSpec(spec);

        // If no activated bundles are found, return an empty list.
        // This is a valid state, not an error.
        if (order == null)
        {
            throw new NotFoundException("Gói hàng không tồn tại");
        }

        // SECURITY CHECK:
        // Has the bundle been activated
        if (!order.IsKeyActivated)
        {
            throw new OperationFailedException("Bạn chưa kích hoạt gói học liệu này");
        }

        // Does the organization that owns this bundle match the user's organization?
        if (order.OrganizationId != request.GetBundleByIdRequestDto.OrganizationId)
        {
            throw new OperationFailedException("Bạn không có quyền truy cập học liệu này.");
        }

        // Map the Order to BundlePayloadDto
        var bundlePayload = order.ToBundlePayloadDto();

        // Return the the bundle
        return bundlePayload;
    }
}
