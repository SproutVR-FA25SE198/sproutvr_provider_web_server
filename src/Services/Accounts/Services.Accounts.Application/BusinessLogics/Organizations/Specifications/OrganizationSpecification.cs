using Common.Application.Helpers;
using Services.Accounts.Application.BusinessLogics.Organizations.Features.GetOrganizations;
using Services.Accounts.Domain.Entities.Organizations;
using Services.Accounts.Domain.Entities.UserAccounts;

namespace Services.Accounts.Application.BusinessLogics.Organizations.Specifications;
public class OrganizationSpecification : BaseSpecification<Organization>
{
    public OrganizationSpecification(GetOrganizationsDto specParams)
        : base(x =>
            (string.IsNullOrEmpty(specParams.Name) || x.Name.Contains(specParams.Name)) &&
            (string.IsNullOrEmpty(specParams.Email) || x.Email!.Contains(specParams.Email)) &&
            (string.IsNullOrEmpty(specParams.PhoneNumber) || x.PhoneNumber!.Contains(specParams.PhoneNumber)) &&
            (string.IsNullOrEmpty(specParams.Address) || x.Address.Contains(specParams.Address)) &&
            (string.IsNullOrEmpty(specParams.MACAddress) || x.MACAddress!.Contains(specParams.MACAddress)) &&
            (string.IsNullOrEmpty(specParams.Status) || Enum.Parse<AccountStatus>(specParams.Status) == x.Status)
        )
    {
        AddOrderBy(x => x.Name);
        ApplyPaging(specParams.PageSize * (specParams.PageIndex - 1), specParams.PageSize);
    }
}

