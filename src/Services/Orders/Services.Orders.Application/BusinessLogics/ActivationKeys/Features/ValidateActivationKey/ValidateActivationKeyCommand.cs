using MediatR;

namespace Services.Orders.Application.BusinessLogics.ActivationKeys.Features.ValidateActivationKey;
public class ValidateActivationKeyCommand(ActivationRequestDto activationRequest) : IRequest<ValidateActivationKeyPayloadDto>
{
    public ActivationRequestDto ActivationRequest { get; } = activationRequest;
}
