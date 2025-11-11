using MediatR;

namespace Services.Orders.Application.BusinessLogics.ActivationKeys.Features.GetBundles;
public class GetBundleByIdQuery(Guid orderId, GetBundleByIdRequestDto requestDto) : IRequest<BundlePayloadDto>
{
    public Guid OrderId { get; set; } = orderId;
    public GetBundleByIdRequestDto GetBundleByIdRequestDto { get; set; } = requestDto;
}
