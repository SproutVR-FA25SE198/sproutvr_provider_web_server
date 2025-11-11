using MediatR;

namespace Services.Orders.Application.BusinessLogics.ActivationKeys.Features.ValidateActivationKey;
public class ValidateActivationKeyCommand(ActivationRequestDto activationRequest) : IRequest<bool>
{
    public ActivationRequestDto ActivationRequest { get; } = activationRequest;
}
