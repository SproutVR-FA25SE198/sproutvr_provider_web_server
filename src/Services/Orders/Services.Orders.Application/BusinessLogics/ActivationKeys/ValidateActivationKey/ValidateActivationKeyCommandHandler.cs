using Common.Application.Abstractions.Data;
using Common.Domain.Exceptions;
using MediatR;
using Services.Orders.Application.BusinessLogics.ActivationKeys.GetBundles;
using Services.Orders.Application.BusinessLogics.Orders.Specifications;
using Services.Orders.Domain.Entities.OrderItems;
using Services.Orders.Domain.Entities.Orders;

namespace Services.Orders.Application.BusinessLogics.ActivationKeys.ValidateActivationKey;
public class ValidateActivationKeyCommandHandler(
    IUnitOfWork uow) : IRequestHandler<ValidateActivationKeyCommand, bool>
{
    public async Task<bool> Handle(ValidateActivationKeyCommand request, CancellationToken cancellationToken)
    {
        // Validate input
        ActivationRequestDto activationRequest = request.ActivationRequest
            ?? throw new OperationFailedException("Thiếu dữ liệu kích hoạt");

        // Check if key is null or empty
        if (string.IsNullOrEmpty(activationRequest.ActivationKey))
        {
            throw new OperationFailedException("Mã kích hoạt đang để trống");
        }

        // Get Order by activation key
        var spec = new OrderSpecification(request.ActivationRequest.ActivationKey);
        Order order = await uow.Repository<Order>().GetEntityWithSpec(spec);

        // Check if key is valid
        if (order == null || order.Status != OrderStatus.Finished)
        {
            throw new OperationFailedException("Mã kích hoạt không hợp lệ");
        }

        // SECURITY CHECK 1: Does the Organization ID that OWNS this key
        // in our database match the Organization ID the user CLAIMS to be?
        if (order.OrganizationId != activationRequest.OrganizationId)
        {
            throw new UnauthorizedAccessException("Bạn không có quyền kích hoạt mã này");
        }

        // SECURITY CHECK 2: Has this key already been activated
        if (order.IsKeyActivated)
        {
            throw new OperationFailedException("Mã này đã được kích hoạt");
        }

        // If all check passed claim the key
        order.IsKeyActivated = true;
        bool result = await uow.SaveChangesAsync(cancellationToken);

        // Return result
        return result;
    }

    /// <summary>
    /// Maps the Order entity to the DTO.
    /// </summary>
    private BundlePayloadDto BuildPayloadAsync(Order order)
    {
        // Prepare map payload list
        var itemPayloadList = new List<MapPayloadDto>();
        foreach (OrderItem item in order.OrderItems)
        {
            // Add map payload which includes download url to the map payload list
            itemPayloadList.Add(new MapPayloadDto
            {
                OrderItemId = item.Id,
                MapName = item.MapName,
                MapCode = item.MapCode,
                ImageUrl = item.ImageUrl,
                DownloadUrl = item.DownloadUrl
            });
        }

        // Prepare main bundle payload
        var payload = new BundlePayloadDto
        {
            OrderId = order.Id.ToString(),
            OrganizationId = order.OrganizationId,
            Maps = itemPayloadList
        };

        // Return the payload
        return payload;
    }
}
