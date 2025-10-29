using MediatR;

namespace Services.Orders.Application.BusinessLogics.ActivationKeys.ValidateActivationKey;
public class ValidateActivationKeyCommand(ActivationRequestDto activationRequest) : IRequest<OrderActivationKeyPayloadDto>
{
    public ActivationRequestDto ActivationRequest { get; } = activationRequest;
}
