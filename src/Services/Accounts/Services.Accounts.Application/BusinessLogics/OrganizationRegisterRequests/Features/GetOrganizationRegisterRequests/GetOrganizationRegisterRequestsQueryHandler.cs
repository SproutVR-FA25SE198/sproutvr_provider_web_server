using Common.Application.Abstractions.Data;
using Common.Application.Helpers;
using MediatR;
using Services.Accounts.Application.BusinessLogics.OrganizationRegisterRequests.Mappings;
using Services.Accounts.Application.BusinessLogics.OrganizationRegisterRequests.Specifications;
using Services.Accounts.Domain.Entities.OrganizationRegisterRequests;

namespace Services.Accounts.Application.BusinessLogics.OrganizationRegisterRequests.Features.GetOrganizationRegisterRequests;
public class GetOrganizationRegisterRequestsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetOrganizationRegisterRequestsQuery, PaginatedResult<OrganizationRegisterRequestDto>>
{
    public async Task<PaginatedResult<OrganizationRegisterRequestDto>> Handle(GetOrganizationRegisterRequestsQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<OrganizationRegisterRequest> mapList = await unitOfWork.Repository<OrganizationRegisterRequest>().ListAsync(new OrganizationRequestSpecification(request.SpecParams));
        
        int totalCount = await unitOfWork.Repository<OrganizationRegisterRequest>().CountAsync(new OrganizationRequestSpecification(request.SpecParams));
        
        List<OrganizationRegisterRequestDto> result = mapList.Any() ? mapList.Select(x => x.ToDto()).ToList() : [];

        return new PaginatedResult<OrganizationRegisterRequestDto>(request.SpecParams.PageIndex, request.SpecParams.PageSize, totalCount, result);
    }
}
