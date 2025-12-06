using Common.Application.Helpers;
using Services.Accounts.Domain.Entities.OrganizationRegisterRequests;

namespace Services.Accounts.Application.BusinessLogics.OrganizationRegisterRequests.Specifications;
public class OrganizationRequestSpecification : BaseSpecification<OrganizationRegisterRequest>
{
    public OrganizationRequestSpecification(string contactEmail, string contactPhone)
       : base(x =>
           x.ContactEmail == contactEmail ||
           x.ContactPhone == contactPhone
       )
    {
    }

    public OrganizationRequestSpecification(Guid id)
        : base(x => x.Id == id)
    {

    }

    public OrganizationRequestSpecification(OrganizationRequestSpecParams specParams)
        : base(x =>
            (string.IsNullOrEmpty(specParams.OrganizationName) || x.OrganizationName.Contains(specParams.OrganizationName)) &&
            (string.IsNullOrEmpty(specParams.ContactPhone) || x.ContactPhone.Contains(specParams.ContactPhone)) &&
            (string.IsNullOrEmpty(specParams.ContactEmail) || x.ContactEmail.Contains(specParams.ContactEmail)) &&
            (string.IsNullOrEmpty(specParams.Address) || x.Address.Contains(specParams.Address)) &&
            (string.IsNullOrEmpty(specParams.ApprovalStatus) || Enum.Parse<ApprovalStatus>(specParams.ApprovalStatus) == x.ApprovalStatus)
        )
    {

    }
}
