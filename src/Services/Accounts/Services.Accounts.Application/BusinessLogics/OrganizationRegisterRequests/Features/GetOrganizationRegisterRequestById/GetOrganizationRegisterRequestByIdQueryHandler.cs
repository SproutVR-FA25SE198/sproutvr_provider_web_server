using Common.Application.Abstractions.Data;
using Common.Domain.Exceptions;
using MediatR;
using Services.Accounts.Application.BusinessLogics.OrganizationRegisterRequests.Mappings;
using Services.Accounts.Application.BusinessLogics.OrganizationRegisterRequests.Specifications;
using Services.Accounts.Domain.Entities.OrganizationRegisterRequests;

namespace Services.Accounts.Application.BusinessLogics.OrganizationRegisterRequests.Features.GetOrganizationRegisterRequestById;
public class GetOrganizationRegisterRequestByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetOrganizationRegisterRequestByIdQuery, OrganizationRegisterRequestDetailsDto>
{
    public async Task<OrganizationRegisterRequestDetailsDto> Handle(GetOrganizationRegisterRequestByIdQuery request, CancellationToken cancellationToken)
    {
        var spec = new OrganizationRequestSpecification(request.Id);
        OrganizationRegisterRequest orgRequest = await unitOfWork.Repository<OrganizationRegisterRequest>().GetEntityWithSpec(spec);

        if (orgRequest == null)
        {
            throw new NotFoundException("Organization Register Request not found!");
        }

        return orgRequest.ToDetailsDto();
    }
}
