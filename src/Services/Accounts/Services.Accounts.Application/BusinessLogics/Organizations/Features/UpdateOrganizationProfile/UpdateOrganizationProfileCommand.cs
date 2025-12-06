using MediatR;

namespace Services.Accounts.Application.BusinessLogics.Organizations.Features.UpdateOrganizationProfile;
public class UpdateOrganizationProfileCommand : IRequest<bool>
{
    public string MACAdress { get; set; }

}
