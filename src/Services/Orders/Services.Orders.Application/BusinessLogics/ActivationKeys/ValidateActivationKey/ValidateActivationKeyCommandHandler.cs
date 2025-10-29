using Common.Application.Abstractions;
using Common.Application.Abstractions.Data;
using Common.Domain.Entities;
using Common.Domain.Exceptions;
using MediatR;
using Services.Orders.Application.BusinessLogics.Orders.Specifications;
using Services.Orders.Domain.Entities.Orders;

namespace Services.Orders.Application.BusinessLogics.ActivationKeys.ValidateActivationKey;
public class ValidateActivationKeyCommandHandler(
    IUnitOfWork uow,
    IUserContext userContext) : IRequestHandler<ValidateActivationKeyCommand, OrderActivationKeyPayloadDto>
{
    public async Task<OrderActivationKeyPayloadDto> Handle(ValidateActivationKeyCommand request, CancellationToken cancellationToken)
    {
        // Check if key is null or empty
        if (string.IsNullOrEmpty(request?.ActivationRequest?.ActivationKey))
        {
            throw new OperationFailedException("Activation Key is required");
        }

        // Get current user
        CurrentUser? userClaims = userContext.GetCurrentUser() ?? throw new UnauthorizedAccessException();

        // Get Order by activation key
        var spec = new OrderSpecification(request.ActivationRequest.ActivationKey);
        Order order = await uow.Repository<Order>().GetEntityWithSpec(spec);

        // Check if key is valid
        if (order == null || order.Status != OrderStatus.Finished)
        {
            throw new OperationFailedException("Invalid or inactive key.");
        }

        // SECURITY CHECK 1: Key must match the logged-in user's org
        if (!Guid.TryParse(userClaims.Id, out Guid userOrgId))
        {
            throw new UnauthorizedAccessException("User's organization ID in their token is invalid.");
        }

        // Compare the two Guid objects directly.
        if (order.OrganizationId != userOrgId)
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

        // Prepare payload
        var payload = new OrderActivationKeyPayloadDto
        {
            OrderId = order.Id.ToString(),
            OrganizationId = order.OrganizationId
#pragma warning disable S1135
            // TODO: ADD DOWNLOAD URLS IN THE PAYLOAD
        };

        // Return payload
        return payload;
    }
}
