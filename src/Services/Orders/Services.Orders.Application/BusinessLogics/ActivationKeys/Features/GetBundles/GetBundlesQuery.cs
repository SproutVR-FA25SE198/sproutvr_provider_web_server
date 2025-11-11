using MediatR;

namespace Services.Orders.Application.BusinessLogics.ActivationKeys.Features.GetBundles;
public class GetBundlesQuery(GetBundlesRequestDto request) : IRequest<List<BundlePayloadDto>>
{
    public GetBundlesRequestDto GetBundlesRequestDto { get; set; } = request;
}
