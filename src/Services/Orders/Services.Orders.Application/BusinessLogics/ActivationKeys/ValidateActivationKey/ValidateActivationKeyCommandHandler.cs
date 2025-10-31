using Common.Application.Abstractions;
using Common.Application.Abstractions.Data;
using Common.Domain.Entities;
using Common.Domain.Exceptions;
using MediatR;
using Services.Orders.Application.BusinessLogics.Orders.Specifications;
using Services.Orders.Domain.Entities.OrderItems;
using Services.Orders.Domain.Entities.Orders;

namespace Services.Orders.Application.BusinessLogics.ActivationKeys.ValidateActivationKey;
public class ValidateActivationKeyCommandHandler(
    IUnitOfWork uow) : IRequestHandler<ValidateActivationKeyCommand, OrderActivationKeyPayloadDto>
{
    public async Task<OrderActivationKeyPayloadDto> Handle(ValidateActivationKeyCommand request, CancellationToken cancellationToken)
    {
        // Validate inpute
        ActivationRequestDto activationRequest = request.ActivationRequest
            ?? throw new OperationFailedException("Activation request is missing.");

        // Check if key is null or empty
        if (string.IsNullOrEmpty(activationRequest.ActivationKey))
        {
            throw new OperationFailedException("Activation Key is required");
        }

        // Get Order by activation key
        var spec = new OrderSpecification(request.ActivationRequest.ActivationKey);
        Order order = await uow.Repository<Order>().GetEntityWithSpec(spec);

        // Check if key is valid
        if (order == null || order.Status != OrderStatus.Finished)
        {
            throw new OperationFailedException("Invalid or inactive key.");
        }

        // SECURITY CHECK 1: Does the Organization ID that OWNS this key
        // in our database match the Organization ID the user CLAIMS to be?
        if (order.OrganizationId != activationRequest.OrganizationId)
        {
            throw new UnauthorizedAccessException("This key is not valid for your organization.");
        }

        // SECURITY CHECK 2: Has this key already been activated
        if (order.IsKeyActivated)
        {
            throw new OperationFailedException("This activation key has already been activated.");
        }

        // If all check passed claim the key
        order.IsKeyActivated = true;
        await uow.SaveChangesAsync(cancellationToken);

        // Build and return payload
        return BuildPayloadAsync(order);
    }

    /// <summary>
    /// Maps the Order entity to the DTO.
    /// </summary>
    private OrderActivationKeyPayloadDto BuildPayloadAsync(Order order)
    {
        // Prepare map payload list
        var itemPayloadList = new List<MapPayloadDto>();
        foreach (OrderItem item in order.OrderItems)
        {
            // Add map payload which includes download url to the map payload list
            itemPayloadList.Add(new MapPayloadDto
            {
                MapId = item.MapId,
                MapName = item.MapName,
                MapCode = item.MapCode,
                ImageUrl = item.ImageUrl,
                DownloadUrl = item.DownloadUrl
            });
        }

        // Prepare main bundle payload
        var payload = new OrderActivationKeyPayloadDto
        {
            OrderId = order.Id.ToString(),
            OrganizationId = order.OrganizationId,
            Maps = itemPayloadList
        };

        // Return the payload
        return payload;
    }
}
