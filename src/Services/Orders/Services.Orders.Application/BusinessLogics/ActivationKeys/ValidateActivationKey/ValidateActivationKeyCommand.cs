using MediatR;
using Services.Orders.Application.BusinessLogics.ActivationKeys.GetBundles;

namespace Services.Orders.Application.BusinessLogics.ActivationKeys.ValidateActivationKey;
public class ValidateActivationKeyCommand(ActivationRequestDto activationRequest) : IRequest<bool>
{
    public ActivationRequestDto ActivationRequest { get; } = activationRequest;
}
