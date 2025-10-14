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
}
