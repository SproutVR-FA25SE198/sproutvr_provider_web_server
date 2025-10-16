using Services.Accounts.Application.BusinessLogics.OrganizationRegisterRequests.Features.CreateOrganizationRegisterRequest;
using Services.Accounts.Application.BusinessLogics.OrganizationRegisterRequests.Features.GetOrganizationRegisterRequestById;
using Services.Accounts.Application.BusinessLogics.OrganizationRegisterRequests.Features.GetOrganizationRegisterRequests;
using Services.Accounts.Domain.Entities.OrganizationRegisterRequests;

namespace Services.Accounts.Application.BusinessLogics.OrganizationRegisterRequests.Mappings;
public static class OrganizationRegisterRequestMappings
{
    public static OrganizationRegisterRequest ToEntity(this CreateOrganizationRequestCommand command)
    {
        var entity = new OrganizationRegisterRequest()
        {
            OrganizationName = command.OrganizationName,
            ContactEmail = command.ContactEmail,
            ContactPhone = command.ContactPhone,
            Address = command.Address,
        };
        return entity;
    }
    public static OrganizationRegisterRequestDto ToDto(this OrganizationRegisterRequest request)
    {
        var dto = new OrganizationRegisterRequestDto()
        {
            Id = request.Id.ToString(),
            OrganizationName = request.OrganizationName,
            ContactEmail = request.ContactEmail,
            ContactPhone = request.ContactPhone,
            Address = request.Address,
            ApprovalStatus = request.ApprovalStatus.ToString(),
        };
        return dto;
    }
    public static OrganizationRegisterRequestDetailsDto ToDetailsDto(this OrganizationRegisterRequest request)
    {
        var dto = new OrganizationRegisterRequestDetailsDto()
        {
            Id = request.Id.ToString(),
            OrganizationName = request.OrganizationName,
            ContactEmail = request.ContactEmail,
            ContactPhone = request.ContactPhone,
            Address = request.Address,
            ApprovalStatus = request.ApprovalStatus.ToString(),
        };
        return dto;
    }
}
